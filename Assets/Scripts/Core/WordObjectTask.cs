using System.Collections.Generic;
using UnityEngine;

namespace RS.Typing.Core {
    public class WordObjectTask : WordObjectBase {
        [SerializeField] private Transform objectiveTransform;
        private ChefTasks _chef;

        protected override void Awake() {
            base.Awake();
            _chef = FindObjectOfType<ChefTasks>();
        }

        protected override void WordComplete() {
            _chef.AddTask(new KeyValuePair<Transform, System.Action>(objectiveTransform, () => {
                Reset();
                Setup();
                base.WordComplete();
            }));
            enabled = false;
        }

        public override bool TryMatch(char ch, bool isFocused) {
            if (char.IsWhiteSpace(Text[Position])) {
                ++Position;
            } 
            if (Text[Position] == ch) {
                ++Position;
                Highlight(Position, true);

                if (Position == Text.Length) {
                    WordComplete();
                    Position = 0;
                }
                return true;
            }
            if (isFocused) Highlight(Position, false);
            return false;
        }
    }
}