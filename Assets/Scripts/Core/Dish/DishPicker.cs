
using UnityEngine;
using UnityEngine.Events;

namespace Core.Dish {
    public class DishPicker : MonoBehaviour {
        [SerializeField] protected DishItemSO dishItem;
        [SerializeField] protected UnityEvent<bool> onDishAvailable;

        protected TrayItemUI itemUI;
        protected int currentStackSize = -1;

        protected void Awake() {
            itemUI = GetComponentInChildren<TrayItemUI>();
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

        protected void RefreshUI() {
            if (!itemUI) return;
            if (!dishItem) {
                itemUI.Reset();
                return;
            }
            name = dishItem.GetItemName() == "" ? dishItem.name : dishItem.GetItemName();
            itemUI.Setup(dishItem);
            itemUI.SetStack(currentStackSize);
        }
    }
}