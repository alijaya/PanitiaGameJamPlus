
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
        }

        private void OnEnable()
        {
            onDishAvailable?.Invoke(dishItem);
            RefreshUI();
        }

        public virtual void AddDish() {
            if (dishItem) Tray.I.AddDish(dishItem);
        }

        public virtual void PickDish() {
            AddDish();
            SetupDish(null);
        }

        public void SetupDish(DishItemSO dishItem) {
            this.dishItem = dishItem;
            this.previewItem = null;
            onDishAvailable?.Invoke(dishItem);
            RefreshUI();
        }

        public void SetupPreview(IngredientItemSO previewItem)
        {
            this.dishItem = null;
            this.previewItem = previewItem;
            RefreshUI();
        }

        public bool HasDish() {
            return dishItem != null;
        }

        //protected void OnValidate() {
        //    RefreshUI();
        //}

        [Button]
        protected void RefreshUI()
        {
            if (dishItem)
            {
                name = dishItem.GetItemName();
            }
            if (itemUI)
            {
                if (dishItem)
                {
                    itemUI.Setup(dishItem);
                    itemUI.SetStack(currentStackSize);
                } else
                {
                    itemUI.Reset();
                }
            }

            if (itemUI2)
            {
                itemUI2.Setup(dishItem ? dishItem : previewItem);
            }
        }
    }
}