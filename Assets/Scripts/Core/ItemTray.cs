using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RS.Typing.Core {
    public class ItemTray : Singleton<ItemTray> {
        [SerializeField] private Item[] itemList;
        [SerializeField] private ItemSO[] itemDataList;
        private readonly Dictionary<ItemSO, Item> itemTray = new Dictionary<ItemSO, Item>();
        public UnityEvent<Dictionary<ItemSO, Item>> ItemTrayUpdated;

        protected override void Awake() {
            base.Awake();
            for (var i = 0; i < itemList.Length; i++) {
                var item = itemList[i];
                var itemData = itemDataList[i];
                
                item.Setup(itemData);
                itemTray[itemData] = item;

                item.StackSizeChanged.AddListener(AnyItemChanged);
            }
        }

        public void AnyItemChanged()
        {
            ItemTrayUpdated.Invoke(itemTray);
        }

        public void AddItemToTray(ItemSO item) {
            itemTray[item].AddStack();
        }

        public bool TryRemoveFromTray(ItemSO item) {
            if (itemTray[item].StackSize == 0) return false;
            itemTray[item].ReduceStack();
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
        
        public Item GetRandomItem() {
            var stackedItem = itemTray.Where(x => x.Value.StackSize > 0);
            return stackedItem.GetRandom().Value;
        }

        public Dictionary<ItemSO, Item> GetItemTray() {
            return itemTray;
        }
    }
}