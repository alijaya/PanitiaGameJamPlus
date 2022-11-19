using System.Collections.Generic;
using UnityEngine;

namespace RS.Typing.Core {
    public class ItemTray : Singleton<ItemTray> {
        [SerializeField] private Item[] itemList;
        [SerializeField] private ItemSO[] itemDataList;
        private readonly Dictionary<ItemSO, Item> itemTray = new ();

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
        }

        public bool TryRemoveFromTray(ItemSO item) {
            if (itemTray[item].StackSize == 0) return false;
            itemTray[item].ReduceStack();
            return true;
        }
    }
}