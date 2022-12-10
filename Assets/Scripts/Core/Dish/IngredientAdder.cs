using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Dish {
    public class IngredientAdder : MonoBehaviour {
        [SerializeField] private IngredientItemSO ingredientItem;
        [SerializeField] private UnityEvent onRecipeAvailable;
        [SerializeField] private UnityEvent onRecipeNotAvailable;

        private RecipeChecker _checker;
        private bool _isBaseIngredient;

        private void Awake() {
            _checker = RecipeChecker.I;
            _isBaseIngredient = _checker.IsBaseIngredient(ingredientItem);
            if (!_isBaseIngredient) {
                _checker.ValidateRecipe += OnValidateRecipe;
            }
        }

        private void OnDestroy() {
            if (!_isBaseIngredient) {
                _checker.ValidateRecipe -= OnValidateRecipe;
            }
        }

        private void Start() {
            if (!_isBaseIngredient) {
                onRecipeNotAvailable?.Invoke();
            }
        }

        private void OnValidateRecipe(IEnumerable<IngredientItemSO> ingredients) {
            if (!ingredients.Contains(ingredientItem)) {
                onRecipeNotAvailable?.Invoke();
            }
            else {
                onRecipeAvailable?.Invoke();
            }
        }

        public void AddIngredient() {
            _checker.AddIngredient(ingredientItem);
        }

        private void OnValidate() {
            if (ingredientItem) {
                name = ingredientItem.GetName() == "" ? ingredientItem.name : ingredientItem.GetName();
            }
        }
    }
}