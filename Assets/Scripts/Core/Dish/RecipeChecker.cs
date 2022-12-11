using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
    
namespace Core.Dish {
    public class RecipeChecker : SingletonMB<RecipeChecker> {
        [SerializeField] private DishRecipeSO[] recipes;
        public UnityEvent<TrayItemSO> onValidRecipe;
        public event Action<IEnumerable<IngredientItemSO>> ValidateRecipe;
        private List<IngredientItemSO> _inputtedIngredients = new ();

        private IEnumerable<IngredientProcessor> _processors;

        private int _ingredientOrder;

        protected override void SingletonAwakened() {
            base.SingletonAwakened();
            _processors = FindObjectsOfType<IngredientProcessor>();
        }

        public bool IsBaseIngredient(IngredientItemSO ingredientItem) {
            var baseIngredients = recipes.Select(recipe => recipe.GetBaseIngredient()).ToList();
            return baseIngredients.Contains(ingredientItem);
        }
        public void AddIngredient(IngredientItemSO ingredientItem) {
            var validRecipes = GetValidRecipes(ingredientItem, out var recipeOutput).ToArray();
            if (!validRecipes.Any()) {
                _inputtedIngredients.Clear();
                return;
            }

            _inputtedIngredients.Add(ingredientItem);
            if (recipeOutput) {
                //onValidRecipe?.Invoke(recipeOutput);

                if (recipeOutput is DishItemSO dish) {
                    ItemTray.I.TryAddItemToTray(dish);
                } else if (recipeOutput is IngredientItemSO ingredient) {
                    var ingredientProcessor = _processors.FirstOrDefault(x => x.IsIngredientValid(ingredient));
                    if (ingredientProcessor) {
                        ingredientProcessor.AddIngredient(ingredient);
                    }
                }

                _ingredientOrder = 0;
                _inputtedIngredients.Clear();
            }
            else {
               NextIngredient(validRecipes);
            }
            
            ItemTray.I.AddIngredientToTray(_inputtedIngredients);
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

        private IEnumerable<DishRecipeSO> GetValidRecipes(IngredientItemSO ingredientItem, out TrayItemSO finalOutput) {
            finalOutput = null;
            var validRecipes = new List<DishRecipeSO>();
            foreach (var recipe in recipes) {
                try {
                    if (recipe.GetRecipe().Check(_ingredientOrder, ingredientItem, out finalOutput)) {
                        validRecipes.Add(recipe);
                    }
                }
                catch (IndexOutOfRangeException e) {
                    Console.WriteLine(e);
                    continue;
                }
                
                if (finalOutput) break;
            }

            return validRecipes;
        }
    }
}