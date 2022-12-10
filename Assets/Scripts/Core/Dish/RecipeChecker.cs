using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Dish {
    public class RecipeChecker : SingletonMB<RecipeChecker> {
        [SerializeField] private DishRecipeSO[] recipes;
        private List<IngredientItemSO> _inputtedIngredients = new ();

        public event Action<IEnumerable<IngredientItemSO>> ValidateRecipe;

        private int _ingredientOrder;
        public bool IsBaseIngredient(IngredientItemSO ingredientItem) {
            var baseIngredients = recipes.Select(recipe => recipe.GetBaseIngredient()).ToList();
            return baseIngredients.Contains(ingredientItem);
        }
        public void AddIngredient(IngredientItemSO ingredientItem) {
            var validRecipes = GetValidRecipes(ingredientItem, out var finalDish).ToArray();
            if (!validRecipes.Any()) {
                _inputtedIngredients.Clear();
                return;
            }

            _inputtedIngredients.Add(ingredientItem);
            
            if (finalDish) {
                Debug.Log(finalDish);
                ItemTray.I.AddItemToTray(finalDish);
                _ingredientOrder = 0;
                _inputtedIngredients.Clear();
            }
            else {
               NextIngredient(validRecipes);
            }
        }
        
        private void NextIngredient(IEnumerable<DishRecipeSO> validRecipes) {
            _ingredientOrder++;
            List<IngredientItemSO> nextIngredients = new();
            foreach (var recipe in validRecipes) {
                var ingredients = recipe.GetIngredientsAt(_ingredientOrder);
                nextIngredients.AddRange(ingredients);
            }
            ValidateRecipe?.Invoke(nextIngredients);
        }

        private IEnumerable<DishRecipeSO> GetValidRecipes(IngredientItemSO ingredientItem, out DishItemSO finalDish) {
            finalDish = null;
            var validRecipes = new List<DishRecipeSO>();
            foreach (var recipe in recipes) {
                try {
                    if (recipe.GetRecipe().Check(_ingredientOrder, ingredientItem, out finalDish)) {
                        validRecipes.Add(recipe);
                    }
                }
                catch (IndexOutOfRangeException e) {
                    Console.WriteLine(e);
                    continue;
                }
                
                if (finalDish) break;
            }

            return validRecipes;
        }
    }
}