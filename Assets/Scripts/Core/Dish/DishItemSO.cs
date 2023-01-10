using UnityEngine;

namespace Core.Dish {
    [CreateAssetMenu(fileName = "Dish Item", menuName = "Dish/Dish", order = 0)]
    public class DishItemSO : TrayItemSO {
        public float difficulty = 1;
        public int price;
    }
}