using System;
using System.Collections.Generic;
using UnityEngine;

namespace LD60Tec.Inventorys
{
    public class InventoryVisualizer : MonoBehaviour
    {
        [SerializeField]
        private InventorySystem _inventory;
        [SerializeField]
        private SpriteRenderer _visual;
        private Queue<GameObject> _queue = new Queue<GameObject>();
        [SerializeField]
        private float _spaceBetween = 1;
        private void Awake()
        {
            _inventory.OnInventoryChanged += UpdateList;
            _inventory.OnRebuild += RebuildList;
        }

        private void RebuildList()
        {
            var inveQueue = _inventory.Inventory;
            foreach (var item in _queue)
            {
                Destroy(item.gameObject);
            }
            _queue.Clear();
            foreach (var item in inveQueue)
            {
                UpdateList(item);
            }
        }

        private void OnDestroy()
        {
            _inventory.OnInventoryChanged -= UpdateList;
            _inventory.OnRebuild -= RebuildList;
        }

        private void UpdateList(ItensSO sO)
        {
            var instance = Instantiate(_visual);
            instance.sprite = sO.ItemSprite;
            _queue.Enqueue(instance.gameObject);
            var sum = _spaceBetween * _queue.Count;
            instance.transform.SetParent(transform, false);
            instance.transform.localPosition = new Vector2(0, sum);
            
        }
    }
}
