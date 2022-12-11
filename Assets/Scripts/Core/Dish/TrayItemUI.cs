using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Dish {
    public class TrayItemUI : MonoBehaviour {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI stackText;

        private void Start() {
            stackText.enabled = false;
        }

        public void Setup(TrayItemSO trayItem) {
            image.enabled = false;
            itemNameText.enabled = false;
            stackText.enabled = false;

            var icon = trayItem.GetItemIconColor();
            var itemName = trayItem.GetItemName();

            if (icon) {
                image.enabled = true;
                image.sprite = icon;
            }
            else {
                itemNameText.enabled = true;
                itemNameText.text = itemName != "" ? itemName : trayItem.name;
            }
        }
    }
}   