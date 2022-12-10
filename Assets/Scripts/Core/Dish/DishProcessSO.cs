using UnityEngine;

namespace Core.Dish {
    [CreateAssetMenu(fileName = "Dish Process", menuName = "Dish/Process", order = 0)]
    public class DishProcessSO : ScriptableObject {
        [SerializeField] private IngredientItemSO ingredientInput;
        [SerializeField] private Stage[] stages;

        public IngredientItemSO GetInput() => ingredientInput;
        public Stage[] GetStages() => stages;

        [System.Serializable]
        public class Stage {    
            [SerializeReference]
            public TrayItemSO output;
            public float timeToProcess;
        }
    }
}