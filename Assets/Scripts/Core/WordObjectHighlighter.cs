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

            const string endTag = "</color>";
            var word = _wordObject.GetWord().Insert(highlightedIndex, endTag);

            if (!isCurrentlyMatched) {
                word = word.Insert(highlightedIndex + endTag.Length,
                    $"<color=#{ColorUtility.ToHtmlStringRGB(wrongHighlightedColor)}>");
            }
            
            text.text = $"<color=#{ColorUtility.ToHtmlStringRGB(highlightedColor)}>{word}";
        }

        public void ResetState() {
            text.text = _wordObject.GetWord();
        }
    }
}