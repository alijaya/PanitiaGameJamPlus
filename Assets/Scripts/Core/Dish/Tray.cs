using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Dish {
    public class Tray : SingletonSceneMB<Tray> {
        public int traySize = 4;

        //[SerializeField] private TraySlot traySlotPrefab;
        //[SerializeField] private Transform trayTransform;

        //private readonly List<TraySlot> _traySlots = new();

        public List<DishItemSO> slots { get; private set; } = new();

        public UnityEvent OnSlotChanged;

        //protected override void SingletonStarted() {
        //    // Instantiate TraySlot
        //    for (var i = 0; i < traySize; i++) {
        //        var traySlot = Instantiate(traySlotPrefab, trayTransform);
        //        traySlot.SlotIndex = i;
        //        _traySlots.Add(traySlot);
        //    }
        //}

        //public void UpdateDisplay()
        //{
        //    for (var i = 0; i < _traySlots.Count; i++)
        //    {
        //        UpdateDisplay(i);
        //    }
        //}

        //public void UpdateDisplay(int index)
        //{
        //    if (index < slots.Count)
        //    {
        //        _traySlots[index].SetDish(slots[index]);
        //    } else
        //    {
        //        _traySlots[index].CleanTray();
        //    }
        //}

        public bool AddDish(DishItemSO dishItem) {
            if (slots.Count < traySize)
            {
                slots.Add(dishItem);
                OnSlotChanged?.Invoke();
                return true;
            } else
            {
                return false;
            }
        }

        //public void AddIngredientToTray(IEnumerable<IngredientItemSO> ingredientItem) {
        //    var slot = _traySlots.First(slot => slot.GetDish() == null);
        //    slot.ShowIngredients(ingredientItem.ToArray());
        //}

        // TODO: Refactor
        //public bool HasDish(DishItemSO dishItem, out TraySlot[] traySlots) {
        //    traySlots = _traySlots.Where(x => x.GetDish() == dishItem).ToArray();
        //    return traySlots.Any();
        //}

        public void RemoveDish(DishItemSO dishItem) {
            slots.Remove(dishItem);
            OnSlotChanged?.Invoke();
        }

        public void RemoveDish(int slotIndex) {
            if (slotIndex < slots.Count)
            {
                slots.RemoveAt(slotIndex);
                OnSlotChanged?.Invoke();
            }
        }
    }
}