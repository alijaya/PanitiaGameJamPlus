using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Core {
    public class Item: MonoBehaviour {
        [SerializeField] private SpriteRenderer itemImage;
        [SerializeField] private TextMeshProUGUI itemCountText;

        private ItemSO item;

        public int StackSize {
            get;
            private set;
        }

        public UnityEvent StackSizeChanged;
        
        public void Setup(ItemSO item) {
            this.item = item;
            itemImage.sprite = item.GetItemIconColor();
            StackSize = 0;
        }

        private void RefreshUI() {
            itemCountText.text = StackSize.ToString();
        }

        public void AddStack() {
            StackSize++;
            RefreshUI();
            StackSizeChanged.Invoke();
        }

        public void ReduceStack() {
            StackSize--;
            RefreshUI();
            StackSizeChanged.Invoke();
        }

        public void SetStackSize(int value) {
            StackSize = value;
            RefreshUI();
            StackSizeChanged.Invoke();
        }
    }
}
