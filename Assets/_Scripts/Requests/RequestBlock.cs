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

            if (!requests.CanComplete(Request)) return;                    // expired, done or game over
            if (!storage.TryRemove(Request.Item, Request.ItemCount)) return; // not enough items, nothing is taken

            requests.TryComplete(Request);
        }

        private void Update()
        {
            Slider.value = Request.Normalized;
        }
    }
}
