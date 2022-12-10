using UnityEngine;

namespace Core.Dish {
    public class TraySlot : MonoBehaviour {
        private DishItemSO _dishItem;

        private void RefreshUI() {
            
        }

        public void AddItem(DishItemSO item) {
            _dishItem = item;
            RefreshUI();
        }

        public DishItemSO GetDish() {
            return _dishItem;
        }

        public void CleanTray() {
            _dishItem = null;
        }
    }
}