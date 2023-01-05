using Core.Dish;
using UnityEngine;

namespace Core.LevelManagement {
    [CreateAssetMenu(fileName = "New Level Progression", menuName = "Level Management/LevelProgression", order = 0)]
    public class LevelProgression : ScriptableObject {
        [SerializeField] private Level[] progressions;

        private int _currentLevel = 0;

        public Level GetLevel(int level) {
            return progressions[level];
        }

        public Level GetCurrentLevel() {
            return progressions[_currentLevel];
        }

        public void SetLevel(int level) {
            _currentLevel = level;
        }
    }
}