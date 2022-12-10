using TMPro;
using UnityEngine;

namespace Core.Dish {
    public class IngredientProcessorUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI text;

        public void OnStagesChanged(TrayItemSO item) {
            text.text = $"Kompor: \n {(item? item.name : "")}";
        }
    }
}