using System;
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
                slot.SlotIndex = i;
                _slots.Add(slot);
            }
        }

        private void AddItemToTray(DishItemSO dishItem) {
            var slot = _slots.First(slot => slot.GetDish() == null);
            slot.AddItem(dishItem);
        }

        public void AddIngredientToTray(IEnumerable<IngredientItemSO> ingredientItem) {
            var slot = _slots.First(slot => slot.GetDish() == null);
            slot.ShowIngredients(ingredientItem.ToArray());
        }

        public bool TryAddItemToTray(DishItemSO dishItemSo) {
            try {
                AddItemToTray(dishItemSo);
                return true;
            }
            catch (InvalidOperationException e) {
                return false;
            }
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

        public void TakeDish(int slotIndex) {
            _slots.First(x => x.SlotIndex == slotIndex).CleanTray();
        }
    }
}