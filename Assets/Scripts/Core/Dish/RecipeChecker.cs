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
        private IngredientItemSO[] _expectedIngredient;
        private RecipeNode _currentNode;
        private CancellationTokenSource _completeCheckCancel;

        private void Start() {
            _expectedIngredient = recipe.GetBaseIngredients().ToArray();
        }

        public bool IsBaseIngredient(IngredientItemSO ingredientItem) {
            return recipe.GetBaseIngredients().Contains(ingredientItem);
        }

        public void AddIngredient(IngredientItemSO ingredientItem) {
            if (_expectedIngredient.Contains(ingredientItem)) {
                //IngredientAdded?.Invoke(ingredientItem);
                if (_currentNode == null) {
                    _currentNode = recipe.GetBaseNode(ingredientItem);
                    NextIngredient();
                    onRecipeStarted?.Invoke();
                    return;
                }
                _currentNode = recipe.GetAllChildren(_currentNode).FirstOrDefault(x => x.GetInput() == ingredientItem);
                if (!_currentNode) {
                    Reset();
                    return;
                }
                //IngredientCombined?.Invoke(_currentNode.output);

                if (_currentNode.IsOutputNode()) {
                    CompleteRecipe(_currentNode.GetOutput()).Forget();
                    return;
                }
                        
                NextIngredient();
            }
            else {
                Reset();
                if (IsBaseIngredient(ingredientItem)) {
                    AddIngredient(ingredientItem);
                }
            }
        }
        
        private void NextIngredient() {
            _expectedIngredient = recipe.GetAllChildren(_currentNode).Select(x => x.GetInput()).ToArray();
            ValidateRecipe?.Invoke(_expectedIngredient);
        }

        private void Reset() {
            _currentNode = null;
            _expectedIngredient = recipe.GetBaseIngredients().ToArray();
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