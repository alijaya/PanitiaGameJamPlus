using System.Collections.Generic;
using UnityEngine;

namespace RS.Typing.Core {
    public class ItemTray : Singleton<ItemTray> {
        [SerializeField] private Item[] itemList;
        [SerializeField] private ItemSO[] itemDataList;
        private readonly Dictionary<ItemSO, Item> itemTray = new Dictionary<ItemSO, Item>();
        public event System.Action<Dictionary<ItemSO, Item>> ItemTrayUpdated;

        protected override void Awake() {
            base.Awake();
            for (var i = 0; i < itemList.Length; i++) {
                var item = itemList[i];
                var itemData = itemDataList[i];
                
                item.Setup(itemData);
                itemTray[itemData] = item;
            }
        }

        public void AddItemToTray(ItemSO item) {
            itemTray[item].AddStack();
            ItemTrayUpdated?.Invoke(itemTray);
        }

        public bool TryRemoveFromTray(ItemSO item) {
            if (itemTray[item].StackSize == 0) return false;
            itemTray[item].ReduceStack();
            ItemTrayUpdated?.Invoke(itemTray);
            return true;
        }

        public bool IsItemInTray(ItemSO item) {
            return (itemTray.ContainsKey(item) && itemTray[item].StackSize > 0);
        }

        public void ClearTray() {
            foreach (var (_, itemObject) in itemTray) {
                itemObject.SetStackSize(0);
            }
        }
    }
}