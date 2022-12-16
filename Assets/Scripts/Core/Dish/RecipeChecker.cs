using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Dish {
    public class RecipeChecker : MonoBehaviour,IIngredientReceiver {
        [SerializeField] private DishRecipeSO recipe;
        public UnityEvent onRecipeStarted;

        public UniTaskFunc completeCheck;
        public UnityEvent<TrayItemSO> onRecipeImmediateComplete;
        public UnityEvent<TrayItemSO> onRecipeComplete;
        public event Action<IEnumerable<IngredientItemSO>> ValidateRecipe;
        public event Action<TrayItemSO> IngredientAdded;
        public event Action<TrayItemSO> IngredientCombined;
        
        private List<IngredientItemSO> _inputtedIngredients = new ();

        private int _ingredientOrder = -1;
        private IngredientItemSO[] _expectedIngredient;
        
        private CancellationTokenSource _completeCheckCancel;

        private void Start() {
            _expectedIngredient = new[] { recipe.GetBaseIngredient() };
        }

        public bool IsBaseIngredient(IngredientItemSO ingredientItem) {
            return recipe.GetBaseIngredient() == ingredientItem;
        }

        public void AddIngredient(IngredientItemSO ingredientItem) {
            if (_expectedIngredient.Contains(ingredientItem)) {
                IngredientAdded?.Invoke(ingredientItem);
                if (_ingredientOrder < 0) {
                    NextIngredient();
                    onRecipeStarted?.Invoke();
                    return;
                }

                if (recipe.GetRecipe().Check(_ingredientOrder, ingredientItem, out var output)) {
                    IngredientCombined?.Invoke(output);
                    if (recipe.GetRecipeStep()-1 == _ingredientOrder) {
                        CompleteRecipe(output).Forget();
                        return;
                    }
                }
                NextIngredient();
            }
            else {
                //reset
                Reset();
                IngredientAdded?.Invoke(null);
                if (IsBaseIngredient(ingredientItem)) {
                    AddIngredient(ingredientItem);
                }
            }
        }
        
        private void NextIngredient() {
            _ingredientOrder++;
            _expectedIngredient = recipe.GetIngredientsAt(_ingredientOrder).ToArray();
            ValidateRecipe?.Invoke(_expectedIngredient);
        }

        private void Reset() {
            _ingredientOrder = -1;
            _expectedIngredient = new[] { recipe.GetBaseIngredient() };
            ValidateRecipe?.Invoke(_expectedIngredient);
        }

        private async UniTask CompleteRecipe(TrayItemSO output) {
            Reset();
            onRecipeImmediateComplete?.Invoke(output);
            _completeCheckCancel ??= new CancellationTokenSource();
            
            var success = true;
            if (completeCheck.target != null) {
                success = ! await completeCheck.Invoke(_completeCheckCancel.Token).SuppressCancellationThrow();
            }
            
            _completeCheckCancel.Dispose();
            _completeCheckCancel = null;
            if (success) onRecipeComplete?.Invoke(output);
        }

    }
}