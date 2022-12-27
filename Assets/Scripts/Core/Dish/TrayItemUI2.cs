using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Dish
{
    public class TrayItemUI2 : MonoBehaviour
    {
        public TrayItemSO trayItem;
        public bool limited = false;
        public int stack = 0;

        [SerializeField] private SpriteRenderer image;
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private StackUI stackUI;

        public void Start()
        {
            UpdateDisplay();
        }

        public void Setup(TrayItemSO trayItem)
        {
            Setup(trayItem, false, 0);
        }

        public void Setup(TrayItemSO trayItem, int stack)
        {
            Setup(trayItem, true, stack);
        }

        public void Setup(TrayItemSO trayItem, bool limited, int stack)
        {
            this.trayItem = trayItem;
            this.limited = limited;
            this.stack = stack;

            UpdateDisplay();
        }

        public void SetStack(int stack)
        {
            this.stack = stack;

            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            if (image) image.enabled = false;
            if (itemNameText) itemNameText.enabled = false;
            if (stackUI) stackUI.gameObject.SetActive(false);

            if (!trayItem) return;

            var icon = trayItem.GetSprite();
            var itemName = trayItem.GetItemName();

            if (icon)
            {
                if (image)
                {
                    image.enabled = true;
                    image.sprite = icon;
                }
            }
            else
            {
                if (itemNameText)
                {
                    itemNameText.enabled = true;
                    itemNameText.text = itemName;
                }
            }

            if (limited)
            {
                if (stackUI)
                {
                    stackUI.gameObject.SetActive(true);
                    stackUI.SetValue(stack);
                }
            }
        }
    }
}