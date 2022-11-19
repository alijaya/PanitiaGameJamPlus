using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RS.Typing.Core {
    public class WordObject : MonoBehaviour {
        public class WordObjectArgs : EventArgs {
            public string Word;
            public bool IsMatch;
            public int HighlightedIndex;
        }
        public static event EventHandler<WordObjectArgs> WordMatched;

        [SerializeField] private TMP_Text text;
        [SerializeField] private UnityEvent wordCompleted;

        private IDisposable _inputCall;

        private string _word;
        private int _highlightedIndex;

        private void Start() {
            Setup();
        }

        private void Setup() {
            _word = WordSpawner.Instance.GetRandomWord(WordDifficult.Normal);
            text.text = _word;
        }

        private void OnEnable() {
            KeyInput.KeyDown += Action;
        }

        private void OnDisable() {
            KeyInput.KeyDown -= Action;
        }

        private void Action(string s, WordObject wordObject) {
            if (wordObject != null && wordObject != this) return;
            
            if (s == "") return;
            if (s != null) {
                AttemptInput(s[0], wordObject);
            }
            else {
                Reset();
            }
        }

        private void AttemptInput(char c, bool hasReference) {
            if (_word[_highlightedIndex] != c) {
                if (hasReference) Error();
                return;
            }

            KeyInput.Instance.lockedWord = this;
            _highlightedIndex++;

            WordMatched?.Invoke(_highlightedIndex > _word.Length ? null: this, new WordObjectArgs{
                Word = _word,
                HighlightedIndex = _highlightedIndex,
                IsMatch = true
            });
            CheckEmpty();
        }
        private void CheckEmpty() {
            if (_highlightedIndex < _word.Length) return;
            wordCompleted?.Invoke();
            Reset();
            Setup();
        }

        private void Reset() {
            _highlightedIndex = 0;
            KeyInput.Instance.lockedWord = null;
            WordMatched?.Invoke(null, new WordObjectArgs{
                Word = _word,
            });
        }

        private void Error() {
            WordMatched?.Invoke(this, new WordObjectArgs{
                Word = _word,
                HighlightedIndex = _highlightedIndex,
                IsMatch = false
            });
        }

        public string GetWord() {
            return _word;
        }
        
    }
}