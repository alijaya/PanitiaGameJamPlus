using UnityEngine;

namespace Core.Dish {
    public class DishPickerRefillable : DishPicker {
        [SerializeField] private int stackSize;

        private int _currentStackSize;
        private void Start() {
            _currentStackSize = stackSize;
        }

        public override void AddDish() {
            if (_currentStackSize <= 0) return;
            if (ItemTray.I.TryAddItemToTray(dishItem)) _currentStackSize--;
        }

        public bool TryRefill() {
            if (_currentStackSize >= stackSize) return false;
            _currentStackSize++;
            return true;

        }
        
    }
}