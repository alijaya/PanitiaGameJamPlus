using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Core.Words {
    [RequireComponent(typeof(WordObject2))]
    public class WordObjectHighlighter2 : MonoBehaviour {

        public Color highlightedColor;
        public Color wrongHighlightedColor;
        public float dim = .3f;
        public float scale = 1.5f;

        private WordObject2 wordObject;
        private TMP_Text text;

        public Transform transformGroup;
        public CanvasGroup canvasGroup;

        private Tween scaleTween;
        private Tween fadeTween;
        private Tween shakeTween;

        private void Awake() {
            wordObject = GetComponent<WordObject2>();
            text = wordObject.text;
        }

        private void OnEnable()
        {
            wordObject.OnStateChanged.AddListener(HandleStateChanged);
        }

        private void OnDisable()
        {
            wordObject.OnStateChanged.RemoveListener(HandleStateChanged);
        }

        public static string WrapColor(string value, Color color) {
            return value == "" ? "" : $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{value}</color>";
        }

        public void HandleStateChanged(WordObject2.WordObjectState state) {

            var word = wordObject.Text;
            var position = wordObject.Position;

            if (state == WordObject2.WordObjectState.OthersMatched)
            {
                fadeTween?.Kill();
                fadeTween = canvasGroup.DOFade(dim, .3f);
            } else
            {
                fadeTween?.Kill();
                fadeTween = canvasGroup.DOFade(1, .3f);
            }

            if (state == WordObject2.WordObjectState.Matched || state == WordObject2.WordObjectState.Unmatched)
            {
                scaleTween?.Kill();
                scaleTween = transformGroup.DOScale(scale, .3f);
            } else
            {
                scaleTween?.Kill();
                scaleTween = transformGroup.DOScale(1, .3f);
            }

            if (state == WordObject2.WordObjectState.Unmatched)
            {
                shakeTween?.Kill(true);
                shakeTween = transformGroup.DOShakePosition(.3f, .2f, 20, 0);
            }

            if (state == WordObject2.WordObjectState.None || state == WordObject2.WordObjectState.OthersMatched)
            {
                // reset
                text.text = word;
            } else
            {
                var isCurrentlyMatched = state != WordObject2.WordObjectState.Unmatched;

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
            
        }
    }
}