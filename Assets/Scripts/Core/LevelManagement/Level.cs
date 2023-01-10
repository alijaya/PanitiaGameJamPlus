using Core.Dish;
using UnityEngine;

namespace Core.LevelManagement {
    [CreateAssetMenu(fileName = "New Level", menuName = "Level Management/Level", order = 0)]
    public class Level : ScriptableObject {
        public int traySize;
        public DishItemSO[] possibleDish;
        public int[] goalThreshold;
        public float shiftDuration;
    }
}