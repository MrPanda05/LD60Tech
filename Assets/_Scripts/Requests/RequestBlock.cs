using LD60Tec.Commons.Singletons;
using LD60Tec.Storage;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LD60Tec.Requests
{
    public class RequestBlock : MonoBehaviour
    {
        [field: SerializeField]
        public Image Render { get; private set; }
        [field: SerializeField]
        public TMP_Text BlockText { get; private set; }
        [field: SerializeField]
        public TMP_Text TextCount { get; private set; }
        [field: SerializeField]
        public Request Request { get; set; }
        [field: SerializeField]
        public Slider Slider { get; private set; }
        [SerializeField]
        private Button _button;
        [SerializeField]
        private GameObject _audioSucess;
        [SerializeField]
        private GameObject _audioFail;

        private void Start()
        {
            _button.onClick.AddListener(TryToComplete);
        }
        private void OnDestroy()
        {
            _button.onClick.RemoveListener(TryToComplete);
        }

        private void TryToComplete()
        {
            var requests = RequestManagers.Instance;
            var storage = StorageManager.Instance;

            if (!requests.CanComplete(Request)) 
            {
                SoundPool.PlayAt(_audioFail, transform.position);
                return;
            }                    
            if (!storage.TryRemove(Request.Item, Request.ItemCount))
            {
                SoundPool.PlayAt(_audioFail, transform.position);
                return;
            }
            SoundPool.PlayAt(_audioSucess, transform.position);
            requests.TryComplete(Request);
        }

        private void Update()
        {
            Slider.value = Request.Normalized;
        }
    }
}
