using System;
using System.Collections.Generic;
using System.Linq;  
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace Core.Dish {
    public class IngredientAdder : MonoBehaviour {
        [SerializeField] private IngredientItemSO ingredientItem;
        [SerializeField] private UnityEvent<IngredientItemSO> onIngredientAdded;

        [SerializeField] private UnityEvent onRecipeAvailable;
        [SerializeField] private UnityEvent onRecipeNotAvailable;

        private bool _isBaseIngredient;
        public bool IsBaseIngredient {
            get => _isBaseIngredient;
            set {
                if (value == false) {
                    onRecipeNotAvailable?.Invoke();
                }
                _isBaseIngredient = value;
            }
        }

        public TrayItemUI2 itemUI2;

        private void OnEnable()
        {
            RestaurantManager.I.RegisterAdder(this);
            RefreshUI();
        }


        public void ValidateRecipe(IEnumerable<IngredientItemSO> ingredients) {
            if (IsBaseIngredient) return;
            if (ingredients.Contains(ingredientItem)) {
                onRecipeAvailable?.Invoke();
            }
            else {
                onRecipeNotAvailable?.Invoke();
            }
        }

        public void AddIngredient() {
            RestaurantManager.I.ValidateItem(ingredientItem);
        }

        [Button]
        protected void RefreshUI()
        {
            if (ingredientItem)
            {
                name = ingredientItem.GetItemName();
            }

            if (itemUI2)
            {
                itemUI2.Setup(ingredientItem);
            }
        }
        public IngredientItemSO GetIngredient() => ingredientItem;

    }
}