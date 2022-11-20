using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RS.Typing.Core {
    public class Item: MonoBehaviour {
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemCountText;
        [SerializeField] private Color highlightedColor;

        public int StackSize {
            get;
            private set;
        }
        
        public void Setup(ItemSO item) {
            itemImage.sprite = item.GetItemIcon();
            StackSize = 0;
        }

        private void RefreshUI() {
            itemCountText.text = StackSize.ToString();
        }

        public void SetHighlight(bool value) {
            itemImage.color = value ? highlightedColor : Color.white;
        }

        public void AddStack() {
            StackSize++;
            RefreshUI();
        }

        public void ReduceStack() {
            StackSize--;
            RefreshUI();
        }

        public void SetStackSize(int value) {
            StackSize = value;
            RefreshUI();
        }
    }
}
