using UnityEngine;
using UnityEngine.UI;

namespace RS.Typing.Core {
    public class WordObjectHighlighter : MonoBehaviour {
        [SerializeField] private Image backgroundTextImage;
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

        private void WordObjectOnWordMatched(object sender, bool isMatch) {
            if (sender is WordObject wordObject && wordObject == _wordObject) {
                backgroundTextImage.color = isMatch ? highlightedColor : wrongHighlightedColor;
            }
            else {
                ResetState();
            }
        }

        public void ResetState() {
            backgroundTextImage.color = Color.black;
        }
    }
}