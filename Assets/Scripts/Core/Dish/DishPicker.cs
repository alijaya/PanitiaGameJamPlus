
using UnityEngine;
using UnityEngine.Events;

namespace Core.Dish {
    public class DishPicker : MonoBehaviour {
        [SerializeField] protected DishItemSO dishItem;
        [SerializeField] protected UnityEvent<bool> onDishAvailable;

        private TrayItemUI _itemUI;

        private void Awake() {
            _itemUI = GetComponentInChildren<TrayItemUI>();
        }

        public virtual void AddDish() {
            if (dishItem) ItemTray.I.TryAddItemToTray(dishItem);
        }

        public virtual void PickDish() {
            AddDish();
            SetupDish(null);
        }

        public void SetupDish(DishItemSO dishItem) {
            this.dishItem = dishItem;
            onDishAvailable?.Invoke(dishItem);
            RefreshUI();
        }

        public bool HasDish() {
            return dishItem != null;
        }

        protected void OnValidate() {
            RefreshUI();
        }

        private void RefreshUI() {
            if (!_itemUI) return;
            if (!dishItem) {
                _itemUI.Reset();
                return;
            }
            name = dishItem.GetItemName() == "" ? dishItem.name : dishItem.GetItemName();
            _itemUI.Setup(dishItem);
        }
    }
}