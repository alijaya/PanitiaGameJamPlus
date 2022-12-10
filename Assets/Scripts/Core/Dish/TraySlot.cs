using System;
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

        public DishItemSO GetDish() {
            return _dishItem;
        }

        public void CleanTray() {
            _dishItem = null;
            RefreshUI();
        }
    }
}