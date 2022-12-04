namespace RS.Typing.Core {
    public class WordObjectPassive : WordObjectBase {
        protected override void WordComplete() {
            Setup();
        }

        public override bool TryMatch(char ch, bool isFocused) {
            if (char.IsWhiteSpace(Text[Position])) {
                Text = Text.Remove(0, 1);
            }
            if (Text[Position] == ch) {
                Text = Text.Remove(0, 1);
                if (Text.Length == 0) {
                    WordComplete();
                    return true;
                }
            }
            text.text = Text;
            return false;
        }
    }
}