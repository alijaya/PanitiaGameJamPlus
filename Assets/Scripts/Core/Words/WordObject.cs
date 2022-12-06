
namespace Core.Words {
    public class WordObject : WordObjectBase {
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