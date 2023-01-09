using UnityEngine;

namespace Core.Dish {
    public class DishPickerRefillable : DishPicker {
        [SerializeField] private int stackSize;

        private void Start() {
            currentStackSize = stackSize;
            RefreshUI();
        }

        public override bool AddDish() {
            if (currentStackSize <= 0) return false;
            if (!base.AddDish()) return false;
            
            currentStackSize--;
            onDishAvailable?.Invoke(stackSize > 0);
            RefreshUI();
            return true;
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