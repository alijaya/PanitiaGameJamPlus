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
            _wordObject.WordMatched += WordObjectOnWordMatched;
        }

        private void OnDestroy() {
            _wordObject.WordMatched -= WordObjectOnWordMatched;
        }

        private void WordObjectOnWordMatched(int highlightedIndex, bool isCurrentlyMatched) {
            if (highlightedIndex == 0) {
                ResetState();
                return;
            }

            //const string endTag = "</color>";
            //var word = _wordObject.GetWord().Insert(highlightedIndex, endTag);

            //if (!isCurrentlyMatched) {
            //    word = word.Insert(highlightedIndex + endTag.Length,
            //        $"<color=#{ColorUtility.ToHtmlStringRGB(wrongHighlightedColor)}>");
            //}

            //text.text = $"<color=#{ColorUtility.ToHtmlStringRGB(highlightedColor)}>{word}";

            var word = _wordObject.GetWord();
            var matchedChars = word.Substring(0, highlightedIndex);
            var nextChar = highlightedIndex < word.Length ? word.Substring(highlightedIndex, 1) : "";
            var restChars = highlightedIndex < word.Length ? word.Substring(highlightedIndex + 1) : "";

            var resultWord = "";
            resultWord += wrapColor(matchedChars, highlightedColor);
            if (isCurrentlyMatched) resultWord += nextChar; 
            else resultWord += wrapColor(nextChar, wrongHighlightedColor);
            resultWord += restChars;

            text.text = resultWord;
        }

        private string wrapColor(string value, Color color)
        {
            if (value == "") return "";
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{value}</color>";
        }

        public void ResetState() {
            text.text = _wordObject.GetWord();
        }
    }
}