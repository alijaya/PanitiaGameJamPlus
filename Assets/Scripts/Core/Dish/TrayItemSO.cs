using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Dish {
    public abstract class TrayItemSO : ScriptableObject {
        [PreviewField]
        [SerializeField] private Sprite itemIcon;
        [PreviewField]
        [SerializeField] private Sprite itemIconColor;

        [SerializeField] private string itemName;
        
        public Sprite GetItemIcon() => itemIcon;
        public Sprite GetItemIconColor() => itemIconColor;
        public string GetItemName() => itemName;
    }    
}
