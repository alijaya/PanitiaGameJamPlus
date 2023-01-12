using Core.Dish;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Core.LevelManagement {
    [CreateAssetMenu(fileName = "New Level", menuName = "Level Management/Level", order = 0)]
    public class Level : ScriptableObject {
        public int traySize;
        public List<DishItemSO> possibleDish;
        public List<int> goals;
        public int maxGoal;
        [InlineEditor]
        public WaveSequenceSO waveSequence;
    }
}