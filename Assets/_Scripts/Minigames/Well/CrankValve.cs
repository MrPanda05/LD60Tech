using LD60Tec.Commons.Singletons;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace LD60Tec.Minigames.Well
{
    /// <summary>
    /// Valve the player grabs with the left mouse button and drags down.
    /// Letting go (or reaching the bottom) makes it spring back up.
    /// Every full push down adds to the fill amount.
    /// Needs a Collider or Collider2D on the same GameObject.
    /// </summary>
    public class CrankValve : MonoBehaviour
    {
        [Header("Angles (degrees, around Z)")]
        [SerializeField]
        private float _defaultAngle = 40;
        [SerializeField]
        private float _maxCrankedAngle = 5;

        [Header("Feel")]
        [SerializeField, Tooltip("Degrees the valve moves per pixel of mouse drag.")]
        private float _degreesPerPixel = 0.25f;
        [SerializeField, Tooltip("Degrees per second when the valve springs back up.")]
        private float _returnSpeed = 90f;

        [Header("Filling")]
        [SerializeField, Tooltip("How much each full crank adds (0 to 1 range total).")]
        private float _fillPerCrank = 0.1f;
        [SerializeField, Tooltip("Valve must come back above this fraction of the travel before it can count again.")]
        [Range(0f, 1f)]
        private float _rearmThreshold = 0.8f;

        public Action<float> OnFillChanged;   // passes the current fill (0..1)
        public Action OnCranked;              // fires once per full crank
        public Action OnFull;

        [field:SerializeField]
        public float Fill { get; private set; }

        private float _currentAngle;
        private bool _isDragging;
        private bool _armed = true;
        private float _dragStartMouseY;
        private float _dragStartAngle;
        [SerializeField] private InputActionAsset _playerActions;
        private InputAction _click;

        [SerializeField]
        private GameObject _audioFullCrank;

        public void Reset()
        {
            Fill = 0;
            _currentAngle = _defaultAngle;
            Apply();
        }
        private void Awake()
        {
            _currentAngle = _defaultAngle;
            Apply();
            _click = _playerActions.FindAction("Click");
        }

        private void OnMouseDown()
        {
            if (Fill >= 1f) return;

            _isDragging = true;
            print("Draggin");
            _dragStartMouseY = Mouse.current.position.ReadValue().y;
            _dragStartAngle = _currentAngle;
        }

        private void OnMouseUp()
        {
            _isDragging = false;
        }

        private void Update()
        {
            if (_isDragging)
            {
                // Dragging down => negative delta => angle decreases toward the cranked angle.
                float deltaY = Mouse.current.position.ReadValue().y - _dragStartMouseY;
                _currentAngle = Mathf.Clamp(
                    _dragStartAngle + deltaY * _degreesPerPixel,
                    _maxCrankedAngle,
                    _defaultAngle);

                if (_armed && Mathf.Approximately(_currentAngle, _maxCrankedAngle))
                {
                    CompleteCrank();
                }
            }
            else
            {
                // Spring back up.
                _currentAngle = Mathf.MoveTowards(_currentAngle, _defaultAngle, _returnSpeed * Time.deltaTime);
            }

            // Re-arm once the valve is high enough again.
            if (!_armed)
            {
                float travel = _defaultAngle - _maxCrankedAngle;
                float rearmAngle = _maxCrankedAngle + travel * _rearmThreshold;
                if (_currentAngle >= rearmAngle)
                {
                    _armed = true;
                }
            }

            Apply();
        }

        private void CompleteCrank()
        {
            print("Full");
            SoundPool.PlayAt(_audioFullCrank, transform.position);
            _armed = false;
            _isDragging = false; // the player has to grab it again, which feels like a real crank

            Fill = Mathf.Clamp01(Fill + _fillPerCrank);
            OnCranked?.Invoke();
            OnFillChanged?.Invoke(Fill);

            if (Fill >= 1f)
            {
                OnFull?.Invoke();
            }
        }

        private void Apply()
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, _currentAngle);
        }
    }
}