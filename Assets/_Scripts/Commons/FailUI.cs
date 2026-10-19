using LD60Tec.LePlayer;
using LD60Tec.Requests;
using System;
using TMPro;
using UnityEngine;

namespace LD60Tec.Commons
{
    public class FailUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;
        private int _failCount = 0;
        private bool _gameOver = false;
        private bool _paused = false;
        [SerializeField]
        private GameObject _gameOverlay;
        [SerializeField]
        private GameObject _pausedOverlay;

        private void Start()
        {
            RequestManagers.Instance.OnRequestFailed += FailIncrease;
            RequestManagers.Instance.OnGameLost += TotalFail;
            Player.OnPaused += Pause;
            Player.OnUnPaused += UnPause;
            _text.text = $"{_failCount}/3";
            _gameOverlay.SetActive(false);
        }

        private void UnPause()
        {
            if (_gameOver) return;
            _paused = false;
            _pausedOverlay.SetActive(false);
        }

        private void Pause()
        {
            if (_gameOver) return;
            _paused = true;
            _pausedOverlay.SetActive(true);
        }

        private void OnDestroy()
        {
            if (RequestManagers.Instance == null) return;
            RequestManagers.Instance.OnRequestFailed -= FailIncrease;
            RequestManagers.Instance.OnGameLost -= TotalFail;
            Player.OnPaused -= Pause;
            Player.OnUnPaused -= UnPause;
        }

        private void TotalFail()
        {
            _gameOverlay.SetActive(true);
        }

        private void FailIncrease(Request request)
        {
            if (_gameOver) return;
            _failCount++;
            _text.text = $"{_failCount}/3";

        }
    }
}
