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
        [SerializeField]
        private GameObject _gameOverlay;

        private void Start()
        {
            RequestManagers.Instance.OnRequestFailed += FailIncrease;
            RequestManagers.Instance.OnGameLost += TotalFail;
            _text.text = $"{_failCount}/3";
            _gameOverlay.SetActive(false);
        }
        private void OnDestroy()
        {
            if (RequestManagers.Instance == null) return;
            RequestManagers.Instance.OnRequestFailed -= FailIncrease;
            RequestManagers.Instance.OnGameLost -= TotalFail;
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
