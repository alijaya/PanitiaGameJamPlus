using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RS.Typing.Core {
    public class WordObject : MonoBehaviour {
        public event Action<int, bool> WordMatched;

        [SerializeField] private TMP_Text text;
        [SerializeField] private UnityEvent wordCompleted;

        private static readonly List<WordObject> Words = new ();
        private static readonly List<WordObject> PrevHighlightedWords = new ();

        private string _word;
        private static string _typedWord;

        private void Start() {
            Words.Add(this);
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

        private void Action(string s) {
            if (string.IsNullOrEmpty(s)) {
                PrevHighlightedWords.Clear();
                _typedWord = "";
                WordMatched?.Invoke(_typedWord.Length, false);
                return;
            }

            var highlightedWord = Words.Where(x => x.GetWord().StartsWith(s)).ToArray();
            if (highlightedWord.Length > 0) {
                AttemptInput(s);
            }
            else {
                if (PrevHighlightedWords.Count > 0) {
                    KeyInput.Instance.ResetText(_typedWord);

                    if (PrevHighlightedWords.Contains(this) && _word.StartsWith(_typedWord)) {
                        WordMatched?.Invoke(_typedWord.Length, false);
                    }
                }
                else {
                    KeyInput.Instance.ResetText();    
                }
            }
        }

        private void AttemptInput(string value) {
            if (_word.StartsWith(value)) {
                _typedWord = value;
                WordMatched?.Invoke(_typedWord.Length, true);
                PrevHighlightedWords.Add(this);
            }

            if (_word.Equals(value)) {
                wordCompleted?.Invoke();
                Reset();
                Setup();
            }
        }
        
        private void Reset() {
            PrevHighlightedWords.Clear();
            _typedWord = "";
            KeyInput.Instance.ResetText();
            WordMatched?.Invoke(_typedWord.Length, false);
        }

        public string GetWord() {
            return _word;
        }

        public bool IsHighlighted() {
            return PrevHighlightedWords.Contains(this);
        }
        
    }
}