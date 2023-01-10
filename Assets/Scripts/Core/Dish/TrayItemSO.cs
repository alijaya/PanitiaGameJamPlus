using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Dish {
    public abstract class TrayItemSO : ScriptableObject
    {
        [SerializeField] private string itemName;

        [PreviewField]
        [SerializeField] private Sprite sprite;

        public string GetItemName() => string.IsNullOrEmpty(itemName) ? name : itemName;
        public Sprite GetSprite() => sprite;
    }    
}
