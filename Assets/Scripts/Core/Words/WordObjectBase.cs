using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace Core.Words {
    [DisallowMultipleComponent]
    public abstract class WordObjectBase : SerializedMonoBehaviour
    {
        public Generator.ITextGenerator TextGenerator;

        [SerializeField] protected TMP_Text text;
        [SerializeField] protected UnityEvent onWordCompleted;

        private WordObjectHighlighter _highlighter;
        public string Text { get; protected set; }
        protected int Position;

        public static event Action<WordObjectBase> AnyNewWordObjectGenerated;
        public static event Action<WordObjectBase> AnyWordObjectDestroyed;

        #region MonobehaviorLifeCycle

        protected virtual void Awake() {
            _highlighter = GetComponent<WordObjectHighlighter>();
        }

        private void Start() {
            Setup();
        }

        private void OnDestroy() {
            AnyWordObjectDestroyed?.Invoke(this);
        }

        #endregion
       
        protected void Setup() {
            enabled = true;
            GenerateText();
            AnyNewWordObjectGenerated?.Invoke(this);
        }

        [Button]
        public void GenerateText(float difficulty = 0)
        {
            Text = TextGenerator?.Generate(difficulty) ?? WordSpawner.InvalidWord;
            text.text = Text;
        }

        protected virtual void WordComplete() {
            onWordCompleted?.Invoke();
            WordPrompt.I.ResetFocused();
        }

        public abstract bool TryMatch(char ch, bool isFocused);

        public void ResetState() {
            Position = 0;
            _highlighter.ResetState();
        }

        public void NotifyError()
        {

        }

        protected void Highlight(int position, bool isCurrentlyMatched) => _highlighter.Highlight(position, isCurrentlyMatched);
    }
}