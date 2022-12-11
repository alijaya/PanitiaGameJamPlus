using TMPro;
using UnityEngine;

namespace Core.Dish {
    public class TraySlot : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI text;
        private DishItemSO _dishItem;
        
        public int SlotIndex { get; set; }

        private void Start() {
            text.text = "";
        }

        private void RefreshUI() {
            text.text = _dishItem? _dishItem.name : "";
        }

        public void AddItem(DishItemSO item) {
            _dishItem = item;
            RefreshUI();
        }

        public void ShowIngredients(IngredientItemSO[] ingredients) {
            text.text = "";
            for (var i = 0; i < ingredients.Length; i++) {
                text.text += ingredients[i].name;

                if (i != ingredients.Length -1) {
                    text.text += " + ";
                }
            } 
        }

        public DishItemSO GetDish() {
            return _dishItem;
        }

        public void CleanTray() {
            _dishItem = null;
            RefreshUI();
        }
    }
}