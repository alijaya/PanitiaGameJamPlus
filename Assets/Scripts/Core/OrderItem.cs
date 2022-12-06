using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class OrderItem : MonoBehaviour
    {
        [SerializeField] private Image itemImage;

        private ItemSO item;

        public int StackSize
        {
            get;
            private set;
        }

        public void Setup(ItemSO item)
        {
            this.item = item;
            itemImage.sprite = item.GetItemIcon();
        }

        public void SetHighlight(bool value)
        {
            itemImage.sprite = value ? item.GetItemIconColor() : item.GetItemIcon();
        }
    }
}
