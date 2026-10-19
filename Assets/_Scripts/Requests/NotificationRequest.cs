using LD60Tec.Commons.Singletons;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LD60Tec.Requests
{
    public class NotificationRequest : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        private Coroutine _notificationCoroutine;
        [SerializeField]
        private GameObject _notificationSound;
        void Start()
        {
            _image.enabled = false;
            RequestManagers.Instance.OnRequestCreated += EnableRequest;
        }
        private void OnDestroy()
        {
            if (RequestManagers.Instance == null) return;
            RequestManagers.Instance.OnRequestCreated -= EnableRequest;
        }
        private void EnableRequest(Request request)
        {
            if (_notificationCoroutine != null)
                StopCoroutine(_notificationCoroutine);

            if (gameObject.activeInHierarchy)
            {
                _notificationCoroutine = StartCoroutine(ShowNotification());
            }
            SoundPool.PlayAt(_notificationSound, transform.position);
        }

        private IEnumerator ShowNotification()
        {
            _image.enabled = true;
            yield return new WaitForSeconds(2f);
            _image.enabled = false;
            _notificationCoroutine = null;
        }
    }
}
