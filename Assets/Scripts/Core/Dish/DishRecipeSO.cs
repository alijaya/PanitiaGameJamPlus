using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Dish {
    [CreateAssetMenu(fileName = "Dish Recipe", menuName = "Dish/Recipe", order = 0)]
    public class DishRecipeSO : ScriptableObject {
        [SerializeField] private Recipe recipe;
        public Recipe GetRecipe() => recipe;

        public IngredientItemSO GetBaseIngredient() => recipe.GetBaseIngredient();

        public IEnumerable<IngredientItemSO> GetIngredientsAt(int ingredientOrder) =>
            recipe.GetIngredientsAt(ingredientOrder);

    }

    [System.Serializable]
    public class Recipe {
        [SerializeField] private Disjunction[] and;
        public bool Check(int ingredientOrder, IngredientItemSO ingredientItem, out TrayItemSO finalOutput) {
            return and[ingredientOrder].Check(ingredientItem, out finalOutput);
        }

        public IngredientItemSO GetBaseIngredient() => and[0].GetBaseIngredient();

        public IEnumerable<IngredientItemSO> GetIngredientsAt(int ingredientOrder) {
            return and[ingredientOrder].GetIngredients();
        }


        [System.Serializable]
        private class Disjunction {
            [SerializeField] private Predicate[] or;

            public bool Check(IngredientItemSO ingredientItem, out TrayItemSO finalOutput) {
                foreach (var predicate in or) {
                    if (predicate.Check(ingredientItem, out finalOutput)) return true;
                }
                finalOutput = null;
                return false;
            }

            public IngredientItemSO GetBaseIngredient() => or[0].GetIngredient();

            public IEnumerable<IngredientItemSO> GetIngredients() {
                return or.Select(predicate => predicate.GetIngredient());
            }
        }
        [System.Serializable]
        private class Predicate {
            [SerializeField] private IngredientItemSO ingredient;
            [SerializeField] private TrayItemSO output;

            public bool Check(IngredientItemSO ingredientItem, out TrayItemSO finalOutput) {
                if (ingredientItem == ingredient) {
                    finalOutput = output;
                    return true;
                }

                finalOutput = null;
                return false;
                
            }

            public IngredientItemSO GetIngredient() => ingredient;
        }    
        
        
    }
}