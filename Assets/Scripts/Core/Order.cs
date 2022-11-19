using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RS.Typing.Core {
    public class Order : MonoBehaviour {
        [SerializeField] private Item itemPrefab;
        [SerializeField] private Transform itemContainerTransform;
        
        [SerializeField] private UnityEvent orderCompleted;

        private readonly List<KeyValuePair<ItemSO, Item>> _orderedItems = new ();

        public void Setup(IEnumerable<ItemSO> items) {
            foreach (var itemSo in items) {
                var item = Instantiate(itemPrefab, itemContainerTransform);
                item.Setup(itemSo);
                _orderedItems.Add(new KeyValuePair<ItemSO, Item>(itemSo, item));
            }
        }

        public void AttemptCompleteOrder() {
            var temp = new List<KeyValuePair<ItemSO, Item>>();
            foreach (var t in _orderedItems) {
                var item = t.Key;
                var itemUI = t.Value;
                if (!ItemTray.Instance.TryRemoveFromTray(item)) continue;
                temp.Add(t);
                Destroy(itemUI.gameObject);
            }

            foreach (var item in temp) {
                _orderedItems.Remove(item);
            }


            if (_orderedItems.Count == 0) {
                orderCompleted?.Invoke();
            }
        }
    }
}