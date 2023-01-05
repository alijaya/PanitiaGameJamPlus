using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Dish {
    public class RecipeChecker : IngredientReceiver {
        public DishRecipeSO recipe;
        public UnityEvent onRecipeStarted;

        public UniTaskFunc completeCheck;
        public UnityEvent<TrayItemSO> onRecipeImmediateComplete;
        public UnityEvent<TrayItemSO> onRecipeComplete;
        public event Action<TrayItemSO> IngredientAdded;
        public event Action<TrayItemSO> IngredientCombined;
        
        private List<IngredientItemSO> _inputtedIngredients = new ();
        private IngredientItemSO[] _expectedIngredient;
        
        private RecipeNode _currentNode;
        private CancellationTokenSource _completeCheckCancel;

        private void OnEnable()
        {
            RestaurantManager.I.OnPossibleDishesUpdated += CheckIngredients;
        }

        private void OnDisable()
        {
            RestaurantManager.I.OnPossibleDishesUpdated -= CheckIngredients;
        }

        private void Start() {
            _expectedIngredient = recipe.GetBaseIngredients().ToArray();
        }

        public override bool IsBaseIngredient(IngredientItemSO ingredientItem) {
            return recipe.GetBaseIngredients().Contains(ingredientItem);
        }

        public override void AddIngredient(IngredientItemSO ingredientItem) {
            if (_expectedIngredient.Contains(ingredientItem)) {
                //IngredientAdded?.Invoke(ingredientItem);
                if (_currentNode == null) {
                    _currentNode = recipe.GetBaseNode(ingredientItem);
                    Debug.Log(_currentNode.GetInput().GetItemName());
                    NextIngredient();
                    onRecipeStarted?.Invoke();
                    return;
                }
                _currentNode = recipe.GetAllChildren(_currentNode).FirstOrDefault(x => x.GetInput() == ingredientItem);
                Debug.Log(_currentNode?.GetInput().GetItemName());
                if (!_currentNode) {
                    ResetState();
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
                ResetState();
                if (IsBaseIngredient(ingredientItem)) {
                    AddIngredient(ingredientItem);
                }
            }
        }
        private void NextIngredient() {
            _expectedIngredient = recipe.GetAllChildren(_currentNode).Select(x => x.GetInput()).ToArray();
            ValidateRecipe();
        }

        private void CheckIngredients(List<DishItemSO> possibleDish) {
            var possibleIngredients = recipe.GetPossibleIngredients(possibleDish).ToArray();

            // TODO: Disable for now
            foreach (var adder in _adders) {
                adder.gameObject.SetActive(possibleIngredients.Contains(adder.GetIngredient()));
            }
        }

        private void ValidateRecipe() {
            foreach (var adder in _adders) {
                adder.ValidateRecipe(_expectedIngredient);
            }
        }

        private void ResetState() {
            _currentNode = null;
            _expectedIngredient = recipe.GetBaseIngredients().ToArray();
            ValidateRecipe();
        }

        private async UniTask CompleteRecipe(TrayItemSO output) {
            ResetState();
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