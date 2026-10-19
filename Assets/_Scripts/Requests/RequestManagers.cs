using LD60Tec.Score;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LD60Tec.Requests
{
    /// <summary>
    /// One customer request. Plain data: the UI reads it, the manager owns it.
    /// </summary>
    [System.Serializable]
    public class Request
    {
        public int Id { get; }
        public ItensSO Item { get; }
        public float Duration { get; }
        public float TimeLeft { get; internal set; }
        public int  ItemCount { get; internal set; }

        /// <summary>1 = just created, 0 = about to expire. Handy for a timer bar.</summary>
        public float Normalized => Duration > 0f ? Mathf.Clamp01(TimeLeft / Duration) : 0f;

        public Request(int id, ItensSO item, float duration, int itemCount)
        {
            Id = id;
            Item = item;
            Duration = duration;
            TimeLeft = duration;
            ItemCount = itemCount;

        }
    }

    /// <summary>
    /// Creates requests over time, expires them, and counts failures.
    /// Keep this on an object that is always active (NOT under the kitchen/minigame objects),
    /// otherwise the timers freeze while the player is in a minigame.
    /// </summary>
    public class RequestManagers : MonoBehaviour
    {
        public static RequestManagers Instance { get; private set; }

        [Header("Start")]
        [SerializeField]
        private bool _autoStart = true;
        [SerializeField, Tooltip("Seconds before the very first request (random between X and Y).")]
        private Vector2 _firstRequestDelay = new Vector2(8f, 15f);

        [Header("Limits")]
        [SerializeField, Min(1), Tooltip("Z: max requests open at the same time.")]
        private int _maxActiveRequests = 3;
        [SerializeField, Min(1), Tooltip("Expired requests allowed before the player loses. 1 = lose on the first one.")]
        private int _maxFailures = 3;

        [Header("Time between requests (seconds)")]
        [SerializeField, Tooltip("Random between X and Y, counted from the moment the previous request was created.")]
        private Vector2 _spawnInterval = new Vector2(15f, 25f);
        [SerializeField, Range(0f, 0.2f), Tooltip("Each completed request shrinks the interval by this fraction (0.03 = 3%).")]
        private float _intervalReductionPerCompleted = 0.03f;
        [SerializeField, Range(0.1f, 1f), Tooltip("The interval never goes below this fraction of its original value.")]
        private float _minIntervalMultiplier = 0.4f;

        [Header("Request lifetime (seconds)")]
        [SerializeField]
        private Vector2 _requestDuration = new Vector2(40f, 60f);
        [Header("Min and max items per request")]
        [SerializeField]
        private Vector2Int _requestMaxCount = new Vector2Int(1, 5);

        [Header("What can be requested")]
        [SerializeField]
        private List<ItensSO> _possibleItems = new List<ItensSO>();


        // ---- Events (plain C# Actions, for the UI) ----
        public event Action<Request> OnRequestCreated;
        public event Action<Request> OnRequestCompleted;
        public event Action<Request> OnRequestFailed;        // expired
        public event Action<int, int> OnFailuresChanged;     // (failures, maxFailures)
        public event Action<int> OnCompletedCountChanged;
        public event Action OnGameLost;

        // ---- State ----
        private readonly List<Request> _active = new List<Request>();
        private readonly List<Request> _expired = new List<Request>();
        private bool _running;
        private float _spawnTimer;
        private int _nextId;
        private int _completed;
        private int _failures;

        public IReadOnlyList<Request> ActiveRequests => _active;
        public int CompletedCount => _completed;
        public int Failures => _failures;
        public int MaxFailures => _maxFailures;
        public bool IsRunning => _running;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Start()
        {
            if (_autoStart) StartRequests();
        }

        private void OnValidate()
        {
            _firstRequestDelay = Ordered(_firstRequestDelay);
            _spawnInterval = Ordered(_spawnInterval);
            _requestDuration = Ordered(_requestDuration);
        }

        private static Vector2 Ordered(Vector2 v) => v.x <= v.y ? v : new Vector2(v.y, v.x);

        // ---- Control ----

        /// <summary>Resets everything and starts the first-request countdown.</summary>
        public void StartRequests()
        {
            _active.Clear();
            _expired.Clear();
            _completed = 0;
            _failures = 0;
            _spawnTimer = Random.Range(_firstRequestDelay.x, _firstRequestDelay.y);
            _running = true;

            OnCompletedCountChanged?.Invoke(_completed);
            OnFailuresChanged?.Invoke(_failures, _maxFailures);
        }

        public void StopRequests()
        {
            _running = false;
        }

        /// <summary>Call this when the player unlocks a new crop.</summary>
        public void AddPossibleItem(ItensSO item)
        {
            if (item != null && !_possibleItems.Contains(item))
            {
                _possibleItems.Add(item);
            }
        }

        public bool HasRequestFor(ItensSO item)
        {
            foreach (var r in _active)
            {
                if (r.Item == item) return true;
            }
            return false;
        }

        /// <summary>
        /// Tries to fulfil a request with this item. If several match, the one closest to expiring wins.
        /// Returns true if a request was completed (the caller should then remove the item from the inventory).
        /// </summary>
        public bool TryComplete(ItensSO item)
        {
            if (!_running || item == null) return false;

            Request match = null;
            foreach (var r in _active)
            {
                if (r.Item == item && (match == null || r.TimeLeft < match.TimeLeft))
                {
                    match = r;
                }
            }
            return match != null && TryComplete(match);
        }

        // ---- Loop ----

        private void Update()
        {
            if (!_running) return;

            TickActiveRequests();
            if (_running) TickSpawner();
        }

        private void TickActiveRequests()
        {
            _expired.Clear();
            foreach (var r in _active)
            {
                r.TimeLeft -= Time.deltaTime;
                if (r.TimeLeft <= 0f) _expired.Add(r);
            }

            // Remove first, then notify, so listeners never see a half-updated list.
            foreach (var r in _expired) _active.Remove(r);

            foreach (var r in _expired)
            {
                if (!_running) break;
                Fail(r);
            }
        }

        private void TickSpawner()
        {
            // The countdown only runs while there is room for another request.
            if (_active.Count >= _maxActiveRequests) return;

            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer > 0f) return;

            Spawn();
            _spawnTimer = NextInterval();
        }

        private void Spawn()
        {
            if (_possibleItems.Count == 0)
            {
                Debug.LogWarning("RequestManagers has no possible items to request.", this);
                return;
            }
            var item = _possibleItems[Random.Range(0, _possibleItems.Count)];
            float duration = Random.Range(_requestDuration.x, _requestDuration.y);
            var request = new Request(++_nextId, item, duration, Random.Range(_requestMaxCount.x, _requestMaxCount.y));

            _active.Add(request);
            OnRequestCreated?.Invoke(request);
            print("New request");
        }

        private float NextInterval()
        {
            float multiplier = Mathf.Max(_minIntervalMultiplier, 1f - _completed * _intervalReductionPerCompleted);
            return Random.Range(_spawnInterval.x, _spawnInterval.y) * multiplier;
        }
        /// <summary>True if this exact request is still open and the game is running.</summary>
        public bool CanComplete(Request request)
        {
            return _running && request != null && _active.Contains(request);
        }

        /// <summary>Completes this specific request instance (what a block's button calls).</summary>
        public bool TryComplete(Request request)
        {
            if (!_running || request == null) return false;
            if (!_active.Remove(request)) return false;   // already completed or expired

            _completed++;
            OnRequestCompleted?.Invoke(request);
            OnCompletedCountChanged?.Invoke(_completed);
            IncreaseDiff();
            Moneymanager.Instance.IncreaseMoney(request.ItemCount * Random.Range(10, 50));
            return true;
        }
        private void IncreaseDiff()
        {
            // Increase the number of items required per request.
            if(_completed % 2 ==  0)
            {
                if(_completed <= 5)
                {
                    _requestMaxCount.x += 1;
                }
                _requestMaxCount.y += 1;
            }

            // Decrease request lifetime, with minimum limits.
            if ((_requestDuration.y - 1) >= 25)
                _requestDuration.y -= 5;

            if ((_requestDuration.x - 1) >= 10)
                _requestDuration.x -= 5;

            // Decrease spawn interval, with a minimum limit.
            if ((_spawnInterval.y - 1) >= 5)
                _spawnInterval.y -= 5;

            if ((_spawnInterval.x - 1) >= 3)
                _spawnInterval.x -= 5;
            // Print updated difficulty.
            Debug.Log(
                $"Successful order! Difficulty increased.\n" +
                $"Required items: {_requestMaxCount.x}-{_requestMaxCount.y}\n" +
                $"Request lifetime: {_requestDuration.x}-{_requestDuration.y} seconds\n" +
                $"Spawn interval: {_spawnInterval.x}-{_spawnInterval.y} seconds"
            );
        }
        private void Fail(Request request)
        {
            _failures++;
            OnRequestFailed?.Invoke(request);
            OnFailuresChanged?.Invoke(_failures, _maxFailures);
            print("Expired");
            if (_failures >= _maxFailures)
            {
                _running = false;
                OnGameLost?.Invoke();
                print("GAME ENDED");
            }
        }
    }
}