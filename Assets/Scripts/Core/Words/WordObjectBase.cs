using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace Core.Words {
    [DisallowMultipleComponent]
    public abstract class WordObjectBase : SerializedMonoBehaviour
    {
        public ITextGenerator TextGenerator;

        //public WordDifficulty difficulty = WordDifficulty.Normal;

        //[Tooltip("Leave it empty to generate random text")]
        //[SerializeField] private string preDefinedText;
        [SerializeField] protected TMP_Text text;
        [SerializeField] protected UnityEvent onWordCompleted;

        private WordObjectHighlighter _highlighter;
        public string Text { get; protected set; }
        protected int Position;

        public static event EventHandler AnyNewWordObjectGenerated;
        public static event EventHandler AnyWordObjectDestroyed;

        #region MonobehaviorLifeCycle

        protected virtual void Awake() {
            _highlighter = GetComponent<WordObjectHighlighter>();
        }

        private void Start() {
            Setup();
        }

        private void OnDestroy() {
            AnyWordObjectDestroyed?.Invoke(this, EventArgs.Empty);
        }

        #endregion
       
        protected void Setup() {
            enabled = true;
            Text = TextGenerator?.Generate() ?? WordSpawner.InvalidWord;
            //Text = string.IsNullOrEmpty(preDefinedText) ? WordSpawner.I.GetRandomWord(difficulty, true) : preDefinedText;
            //if (Text == null) Text = "aligg";
            text.text = Text;
            AnyNewWordObjectGenerated?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void WordComplete() {
            onWordCompleted?.Invoke();
            WordPrompt.I.ResetFocused();
        }

        public abstract bool TryMatch(char ch, bool isFocused);

        public void Reset() {
            Position = 0;
            _highlighter.ResetState();
        }

        protected void Highlight(int position, bool isCurrentlyMatched) => _highlighter.Highlight(position, isCurrentlyMatched);
    }
}