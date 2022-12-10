using UnityEngine;

namespace Core.Dish {
    public class DishPicker : MonoBehaviour {
        [SerializeField] protected DishItemSO dishItem;


        public virtual void AddDish() {
            ItemTray.I.TryAddItemToTray(dishItem);
        }

    }
}