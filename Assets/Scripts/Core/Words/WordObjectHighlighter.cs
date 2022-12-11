using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Core.Words {
    [RequireComponent(typeof(WordObject))]
    public class WordObjectHighlighter : MonoBehaviour {

        public Color highlightedColor;
        public Color wrongHighlightedColor;
        public float dim = .3f;
        public float scale = 1.5f;

        private WordObject wordObject;
        private TMP_Text text;

        public Transform transformGroup;
        public CanvasGroup canvasGroup;

        private Tween scaleTween;
        private Tween fadeTween;
        private Tween shakeTween;

        private void Awake() {
            wordObject = GetComponent<WordObject>();
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

        public void HandleStateChanged(WordObject.WordObjectState state) {
            if (!this || !isActiveAndEnabled || QuitUtil.isQuitting) return;

            var word = wordObject.Text;
            var position = wordObject.Position;

            // fade if others matched, disabled, or completed
            if (state == WordObject.WordObjectState.OthersMatched || state == WordObject.WordObjectState.Disabled || state == WordObject.WordObjectState.Completed)
            {
                fadeTween?.Kill();
                fadeTween = canvasGroup.DOFade(dim, .3f);
            } else
            {
                fadeTween?.Kill();
                fadeTween = canvasGroup.DOFade(1, .3f);
            }

            if (state == WordObject.WordObjectState.Matched || state == WordObject.WordObjectState.Unmatched)
            {
                scaleTween?.Kill();
                scaleTween = transformGroup.DOScale(scale, .3f);
            } else
            {
                scaleTween?.Kill();
                scaleTween = transformGroup.DOScale(1, .3f);
            }

            if (state == WordObject.WordObjectState.Unmatched)
            {
                shakeTween?.Kill(true);
                shakeTween = transformGroup.DOShakePosition(.3f, .2f, 20, 0);
            }

            if (state == WordObject.WordObjectState.None || state == WordObject.WordObjectState.OthersMatched)
            {
                // reset
                text.text = word;
            } else
            {
                var isCurrentlyMatched = state != WordObject.WordObjectState.Unmatched;

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