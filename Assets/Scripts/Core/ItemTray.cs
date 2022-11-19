using System.Collections.Generic;
using UnityEngine;

namespace RS.Typing.Core {
    public class ItemTray : Singleton<ItemTray> {
        [SerializeField] private ItemSO[] itemList;
        [SerializeField] private Item itemUIPrefab;
        private readonly Dictionary<ItemSO, Item> itemTray = new ();

        protected override void Awake() {
            base.Awake();
            foreach (var itemSo in itemList) {
                var itemUI = Instantiate(itemUIPrefab, transform);
                itemUI.Setup(itemSo);
                itemTray.Add(itemSo, itemUI);
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