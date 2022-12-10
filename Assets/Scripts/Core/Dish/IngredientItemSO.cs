using UnityEngine;

namespace Core.Dish {
    [CreateAssetMenu(fileName = "Ingredient Item", menuName = "Items/Ingredient", order = 0)]
    public class IngredientItemSO : ScriptableObject {
        [SerializeField] private string ingredientName;
        public string GetName() => ingredientName;
    }
}