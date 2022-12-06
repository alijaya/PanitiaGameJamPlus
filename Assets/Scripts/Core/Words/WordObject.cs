using Cysharp.Threading.Tasks;
using System;

namespace Core.Words {
    public class WordObject : WordObjectBase {
        public Func<UniTask> CompleteCheck;

        protected override void WordComplete()
        {
            _WordComplete().Forget();
        }

        private async UniTask _WordComplete()
        {
            if (CompleteCheck != null)
            {
                await CompleteCheck();
            }
            onWordCompleted?.Invoke();
            ResetState();
            Setup();
            WordPrompt.I.ResetFocused();
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