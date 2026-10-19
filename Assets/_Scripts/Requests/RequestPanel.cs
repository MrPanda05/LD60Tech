using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LD60Tec.Requests
{
    public class RequestPanel : MonoBehaviour
    {
        [SerializeField]
        private RequestBlock _prefabBlock;

        [SerializeField]
        private List<RequestBlock> _requests = new();

        private void Start()
        {
            print("I should run");
            var manager = RequestManagers.Instance;
            manager.OnRequestCreated += AddRequest;
            manager.OnRequestCompleted += RemoveRequest;
            manager.OnRequestFailed += RemoveRequest;
        }

        private void OnDestroy()
        {
            var manager = RequestManagers.Instance;
            if (manager == null) return; // already torn down, nothing to unsubscribe from

            manager.OnRequestCreated -= AddRequest;
            manager.OnRequestCompleted -= RemoveRequest;
            manager.OnRequestFailed -= RemoveRequest;
        }

        private void RemoveRequest(Request request)
        {
            for (int i = _requests.Count - 1; i >= 0; i--)
            {
                var block = _requests[i];

                if (block == null)
                {
                    _requests.RemoveAt(i);
                    continue;
                }

                if (block.Request == request)
                {
                    _requests.RemoveAt(i);
                    Destroy(block.gameObject);
                    return;
                }
            }
        }

        private void AddRequest(Request request)
        {
            print("Creating request UI");
            var instance = Instantiate(_prefabBlock);
            _requests.Add(instance);
            instance.Request = request;
            instance.BlockText.text = request.Item.ItemName;
            instance.Render.sprite = request.Item.ItemSprite;
            instance.TextCount.text = $"x{request.ItemCount.ToString()}";
            instance.transform.SetParent(transform, false);
        }
    }
}
