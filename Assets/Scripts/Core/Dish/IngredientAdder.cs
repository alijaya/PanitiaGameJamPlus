using System;
using System.Collections.Generic;
using System.Linq;  
using UnityEngine;
using UnityEngine.Events;

namespace Core.Dish {
    public class IngredientAdder : MonoBehaviour {
        [SerializeField] private IngredientItemSO ingredientItem;
        [SerializeField] private UnityEvent<IngredientItemSO> onIngredientAdded;

        [SerializeField] private UnityEvent onRecipeAvailable;
        [SerializeField] private UnityEvent onRecipeNotAvailable;

        private IIngredientReceiver _receiver;
        private bool _isBaseIngredient;

        private void Awake() {
            _receiver = GetComponentInParent<IIngredientReceiver>();
            _isBaseIngredient = _receiver.IsBaseIngredient(ingredientItem);
            if (!_isBaseIngredient && _receiver is RecipeChecker checker) {
                checker.ValidateRecipe += OnValidateRecipe;
            }
        }
        private void OnDestroy() {
            if (!_isBaseIngredient && _receiver is RecipeChecker checker) {
                checker.ValidateRecipe += OnValidateRecipe;
            }
        }

        private void Start() {
            if (!_isBaseIngredient) {
                onRecipeNotAvailable?.Invoke();
            }
        }

        private void OnValidateRecipe(IEnumerable<IngredientItemSO> ingredients) {
            if (ingredients.Contains(ingredientItem)) {
                onRecipeAvailable?.Invoke();
            }
            else {
                onRecipeNotAvailable?.Invoke();
            }
        }

        public void AddIngredient() {
            _receiver.AddIngredient(ingredientItem);
        }

        public IngredientItemSO GetIngredient() => ingredientItem;

        private void OnValidate() {
            if (ingredientItem) {
                name = ingredientItem.GetItemName() == "" ? ingredientItem.name : ingredientItem.GetItemName();
                GetComponentInChildren<TrayItemUI>().Setup(ingredientItem);
            }
        }

    }
}