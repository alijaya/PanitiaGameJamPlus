using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RS.Typing.Core {
    public class Item: MonoBehaviour {
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemCountText;

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

        public void AddStack() {
            StackSize++;
            RefreshUI();
        }

        public void ReduceStack() {
            StackSize--;
            RefreshUI();
        }
    }
}
