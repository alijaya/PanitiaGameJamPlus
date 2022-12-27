using UnityEngine;

namespace Core.Dish {
    public class TraySlot : MonoBehaviour {
        private DishItemSO _dishItem;
        private TrayItemUI _itemUI;
        public int SlotIndex { get; set; }

        private void Awake() {
            _itemUI = GetComponentInChildren<TrayItemUI>();
        }

        public void SetDish(DishItemSO item) {
            _dishItem = item;
            
            _itemUI.Setup(item);
        }

        public DishItemSO GetDish()
        {
            return _dishItem;
        }

        //public void ShowIngredients(IngredientItemSO[] ingredients) {
        //    /*text.text = "";
        //    for (var i = 0; i < ingredients.Length; i++) {
        //        text.text += ingredients[i].name;

        //        if (i != ingredients.Length -1) {
        //            text.text += " + ";
        //        }
        //    }*/ 
        //}

        public void CleanTray() {
            _dishItem = null;
            _itemUI.Reset();
        }
    }
}