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
            WordObject.WordMatched += WordObjectOnWordMatched;
        }

        private void OnDestroy() {
            WordObject.WordMatched -= WordObjectOnWordMatched;
        }

        private void WordObjectOnWordMatched(object sender, WordObject.WordObjectArgs wordObjectArgs) {
            if (sender is WordObject wordObject && wordObject == _wordObject) {
                const string endTag = "</color>";
                var word = wordObjectArgs.Word.Insert(wordObjectArgs.HighlightedIndex, endTag);
                if (!wordObjectArgs.IsMatch) {
                    word = word.Insert(wordObjectArgs.HighlightedIndex + endTag.Length, $"<color=#{ColorUtility.ToHtmlStringRGB(wrongHighlightedColor)}>");
                }
                
                text.text = $"<color=#{ColorUtility.ToHtmlStringRGB(highlightedColor)}>{word}";
            }
            else {
                ResetState();
            }
        }

        public void ResetState() {
            text.text = _wordObject.GetWord();
        }
    }
}