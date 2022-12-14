using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Dish {
    public class RecipeChecker : MonoBehaviour {
        [SerializeField] private DishRecipeSO recipe;
        public UnityEvent onRecipeStarted;
        public UnityEvent<TrayItemSO> onRecipeComplete;
        public event Action<IEnumerable<IngredientItemSO>> ValidateRecipe;
        public event Action<TrayItemSO> IngredientAdded;
        public event Action<TrayItemSO> IngredientCombined;
        
        private List<IngredientItemSO> _inputtedIngredients = new ();

        private int _ingredientOrder = -1;

        public bool IsBaseIngredient(IngredientItemSO ingredientItem) {
            return recipe.GetBaseIngredient() == ingredientItem;
        }

        public void AddIngredient(IngredientItemSO ingredientItem) {
            if (_ingredientOrder >= 0 && !IsBaseIngredient(ingredientItem)) {
                if (recipe.GetRecipe().Check(_ingredientOrder, ingredientItem, out var finalOutput)) {
                    
                    IngredientCombined?.Invoke(finalOutput);
                    if (recipe.GetRecipeStep() == _ingredientOrder + 1) {
                        onRecipeComplete?.Invoke(finalOutput);
                    
                        _ingredientOrder = -1;
                        _inputtedIngredients.Clear();
                        ValidateRecipe?.Invoke(new List<IngredientItemSO>());
                        return;
                    }
                }
            } else if (IsBaseIngredient(ingredientItem)) {
                onRecipeStarted?.Invoke();
            }

            IngredientAdded?.Invoke(ingredientItem);
            NextIngredient();
        }
        
        private void NextIngredient() {
            _ingredientOrder++;
            var nextIngredients = recipe.GetIngredientsAt(_ingredientOrder);
            ValidateRecipe?.Invoke(nextIngredients);
        }

    }
}