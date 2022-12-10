using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Dish {
    public class ItemTray : SingletonMB<ItemTray> {
        [SerializeField] private int traySize;
        [SerializeField] private TraySlot traySlotPrefab;
        [SerializeField] private Transform trayTransform;

        private readonly List<TraySlot> _slots = new();

        protected override void SingletonStarted() {
            base.SingletonStarted();
            for (var i = 0; i < traySize; i++) {
                var slot = Instantiate(traySlotPrefab, trayTransform);
                _slots.Add(slot);
            }
        }

        public void AddItemToTray(DishItemSO dishItem) {
            var slot = _slots.First(slot => slot.GetDish() == null);
            if (slot == null) return;
            
            slot.AddItem(dishItem);
        }

        public bool HasDish(DishItemSO dishItem, out TraySlot[] traySlots) {
            traySlots = _slots.Where(x => x.GetDish() == dishItem).ToArray();
            return traySlots.Any();
        }

        public void TakeDish(DishItemSO dishItem) {
            if (HasDish(dishItem, out var traySlots)) {
                traySlots[0].CleanTray();
            }
        }
    }
}