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

        public IngredientReceiver receiver;

        private IngredientReceiver _receiver;
        private bool _isBaseIngredient;

        public TrayItemUI2 itemUI2;

        private void Awake() {
            if (receiver != null)
            {
                _receiver = receiver;
            } else
            {
                _receiver = GetComponentInParent<IngredientReceiver>();
            }
            _isBaseIngredient = _receiver.IsBaseIngredient(ingredientItem);
            if (!_isBaseIngredient && _receiver is RecipeChecker checker) {
                checker.ValidateRecipe += OnValidateRecipe;
            }
        }

        private void OnEnable()
        {
            RefreshUI();
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
            if (!ingredients.Contains(ingredientItem)) {
                onRecipeNotAvailable?.Invoke();
            }
            else {
                onRecipeAvailable?.Invoke();
            }
        }

        public void AddIngredient() {
            _receiver.AddIngredient(ingredientItem);
        }

        //private void OnValidate() {
        //    if (ingredientItem) {
        //        name = ingredientItem.GetItemName() == "" ? ingredientItem.name : ingredientItem.GetItemName();
        //        GetComponentInChildren<TrayItemUI>().Setup(ingredientItem);
        //    }
        //}

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
    }
}