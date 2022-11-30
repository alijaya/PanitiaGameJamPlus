using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RS.Typing.Core {
    public class WordObject : MonoBehaviour {
        public WordDifficulty difficulty = WordDifficulty.Normal;

        [Tooltip("Leave it empty to generate random text")]
        [SerializeField] private string preDefinedText;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Transform objectiveTransform;
        [SerializeField] private UnityEvent onWordCompleted;

        private ChefTasks _chef;
        private WordObjectHighlighter _highlighter; //shouldn't referencing this
        public string Text { get; private set; }
        public static event EventHandler AnyNewWordObjectGenerated;
        public static event EventHandler AnyWordObjectDestroyed;

        private void Awake() {
            _chef = FindObjectOfType<ChefTasks>();
            _highlighter = GetComponent<WordObjectHighlighter>();
        }

        private void Start() {
            Setup();
        }

        private void OnDestroy() {
            AnyWordObjectDestroyed?.Invoke(this, EventArgs.Empty);
        }

        private void Setup() {
            Text = string.IsNullOrEmpty(preDefinedText) ? WordSpawner.I.GetRandomWord(difficulty, true) : preDefinedText;
            if (Text == null) Text = "aligg";
            text.text = Text;
            AnyNewWordObjectGenerated?.Invoke(this, EventArgs.Empty);
        }
        public void WordComplete() {
            Reset();
            Setup();
            _chef.AddTask(new KeyValuePair<Transform, Action>(objectiveTransform, () => {
                onWordCompleted?.Invoke();
            }));
        }

        public void Reset() {
            _highlighter.ResetState();
        }

        public void Highlight(int position, bool isCurrentlyMatched) => _highlighter.Highlight(position, isCurrentlyMatched);
    }
}