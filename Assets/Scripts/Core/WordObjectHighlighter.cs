using TMPro;
using UnityEngine;

namespace RS.Typing.Core {
    public class WordObjectHighlighter : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI text;
        
        [SerializeField] private Color highlightedColor;
        [SerializeField] private Color wrongHighlightedColor;

        private WordObject _wordObject;

        private void Awake() {
            _wordObject = GetComponent<WordObject>();
        }
        private string WrapColor(string value, Color color) {
            return value == "" ? "" : $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{value}</color>";
        }

        public void Highlight(int position, bool isCurrentlyMatched) {
            var word = _wordObject.Text;
            var matchedChars = word[..position];
            var nextChar = position < word.Length ? word.Substring(position, 1) : "";
            var restChars = position < word.Length ? word[(position + 1)..] : "";
            
            var resultWord = "";
            resultWord += WrapColor(matchedChars, highlightedColor);
            if (isCurrentlyMatched) resultWord += nextChar; 
            else resultWord += WrapColor(nextChar, wrongHighlightedColor);
            resultWord += restChars;
            text.text = resultWord;
        }

        public void ResetState() {
            text.text = _wordObject.Text;
        }
    }
}