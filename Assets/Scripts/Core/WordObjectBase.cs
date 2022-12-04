using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RS.Typing.Core {
    [DisallowMultipleComponent]
    public abstract class WordObjectBase : MonoBehaviour {
        public WordDifficulty difficulty = WordDifficulty.Normal;

        [Tooltip("Leave it empty to generate random text")]
        [SerializeField] private string preDefinedText;
        [SerializeField] protected TMP_Text text;
        [SerializeField] protected UnityEvent onWordCompleted;

        private WordObjectHighlighter _highlighter; //shouldn't referencing this
        public string Text { get; protected set; }
        protected int Position;

        public static Action OnCompleted;
        
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
            Text = string.IsNullOrEmpty(preDefinedText) ? WordSpawner.I.GetRandomWord(difficulty, true) : preDefinedText;
            if (Text == null) Text = "aligg";
            text.text = Text;
            AnyNewWordObjectGenerated?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void WordComplete() {
            onWordCompleted?.Invoke();
            OnCompleted?.Invoke();
        }

        public abstract bool TryMatch(char ch, bool isFocused);

        public void Reset() {
            Position = 0;
            _highlighter.ResetState();
        }

        protected void Highlight(int position, bool isCurrentlyMatched) => _highlighter.Highlight(position, isCurrentlyMatched);
    }
}