using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace Core.Dish {
    public class DishPicker : MonoBehaviour {
        [SerializeField] protected DishItemSO dishItem;
        [SerializeField] protected UnityEvent<bool> onDishAvailable;

        private IngredientItemSO previewItem;

        protected TrayItemUI itemUI;
        protected int currentStackSize = -1;

        public TrayItemUI2 itemUI2;

        protected void Awake() {
            itemUI = GetComponentInChildren<TrayItemUI>();
            if (dishItem) RestaurantManager.I.OnPossibleDishesUpdated += SetActivePossibleDish;
        }

        protected void OnDestroy() {
            if (dishItem) RestaurantManager.I.OnPossibleDishesUpdated -= SetActivePossibleDish;
        }

        private void SetActivePossibleDish(List<DishItemSO> possibleDishes) {
            gameObject.SetActive(possibleDishes.Contains(dishItem));
        }

        private void OnEnable() {
            onDishAvailable?.Invoke(dishItem);
            RefreshUI();
        }

        public virtual bool AddDish() {
            return dishItem && RestaurantManager.I.ValidateItem(dishItem);
        }

        public virtual void PickDish() {
            AddDish();
            SetupDish(null);
        }

        public void SetupDish(DishItemSO dishItem) {
            this.dishItem = dishItem;
            previewItem = null;
            onDishAvailable?.Invoke(dishItem);
            RefreshUI();
        }

        public void SetupDish(TrayItemSO trayItem) {
            if (trayItem is DishItemSO dishItem) {
                SetupDish(dishItem);
            }
        }

        public void SetupPreview(IngredientItemSO previewItem) {
            this.dishItem = null;
            this.previewItem = previewItem;
            RefreshUI();
        }

        public bool HasDish() {
            return dishItem != null;
        }

        [Button]
        protected void RefreshUI() {
            if (dishItem) {
                name = dishItem.GetItemName();
            }

            if (itemUI) {
                if (dishItem) {
                    itemUI.Setup(dishItem);
                    itemUI.SetStack(currentStackSize);
                }
                else {
                    itemUI.Reset();
                }
            }

            if (itemUI2) {
                itemUI2.Setup(dishItem ? dishItem : previewItem);
            }
        }
    }
}