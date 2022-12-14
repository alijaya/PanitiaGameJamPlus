using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Dish {
    public class RecipeVisualizer : MonoBehaviour {
        [SerializeField] private Image[] itemImages = new Image[2];

        private readonly List<TrayItemSO> _current = new();
        private RecipeChecker _checker;
        private TrayItemSO _currentOutput;


        private void Awake() {
            _checker = GetComponentInParent<RecipeChecker>();
            _checker.IngredientAdded += CheckerOnIngredientAdded;
            _checker.IngredientCombined += CheckerOnIngredientCombined;
        }

        private void OnDestroy() {
            _checker.IngredientAdded -= CheckerOnIngredientAdded;
            _checker.IngredientCombined -= CheckerOnIngredientCombined;
        }

        private void CheckerOnIngredientCombined(TrayItemSO output) {
            _currentOutput = output;
        }

        private void CheckerOnIngredientAdded(TrayItemSO item) {
            _current.Add(item);
            RefreshUI();
            if (_current.Count == 2) {
                Combine();
            }
        }

        private void Combine() {
            _current.Clear();
            _current.Add(_currentOutput);
            
            RefreshUI();
        }

        private void RefreshUI() {
            for (var i = 0; i < itemImages.Length -1; i++) {
                itemImages[i].sprite = _current[i].GetItemIcon();
            }
        }
    }
}