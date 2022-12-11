using UnityEngine;

namespace Core.Dish {
    public class DishPicker : MonoBehaviour {
        [SerializeField] protected DishItemSO dishItem;

        public virtual void AddDish() {
            ItemTray.I.TryAddItemToTray(dishItem);
        }

        protected void OnValidate() {
            if (dishItem) {
                name = dishItem.GetItemName() == "" ? dishItem.name : dishItem.GetItemName();
                GetComponentInChildren<TrayItemUI>().Setup(dishItem);
            }
        }
    }
}