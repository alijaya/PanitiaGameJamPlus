using UnityEngine;

namespace Core.Dish {
    public class DishPickerRefillable : DishPicker {
        [SerializeField] private int stackSize;

        private void Start() {
            currentStackSize = stackSize;
            RefreshUI();
        }

        public override void AddDish() {
            if (currentStackSize <= 0) return;
            if (Tray.I.AddDish(dishItem)) currentStackSize--;
            onDishAvailable?.Invoke(stackSize > 0);
            RefreshUI();
        }

        public bool TryRefill() {
            if (currentStackSize >= stackSize) return false;
            currentStackSize++;
            onDishAvailable?.Invoke(stackSize > 0);
            RefreshUI();
            return true;
        }
        
        
        
    }
}