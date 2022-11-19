using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityAtoms.BaseAtoms;

namespace RS.Typing.Core {
    public class WordObject : MonoBehaviour {
        public event Action<int, bool> WordMatched;

        [SerializeField] private TMP_Text text;
        [SerializeField] private UnityEvent wordCompleted;

        private string _word;
        private static string _typedWord;

        private void Start() {
            GlobalRef.I.Words.Add(this.gameObject);
            Setup();
        }

        private void Setup() {
            _word = WordSpawner.I.GetRandomWord(WordDifficulty.Normal, true);
            text.text = _word;
        }

        private void OnDestroy()
        {
            GlobalRef.I.Words.Remove(this.gameObject);
        }

        private void OnEnable() {
            KeyInput.KeyDown += Action;
        }

        private void OnDisable() {
            KeyInput.KeyDown -= Action;
        }

        private void Action(string s) {
            if (string.IsNullOrEmpty(s)) {
                GlobalRef.I.PrevHighlightedWords.Clear();
                _typedWord = "";
                WordMatched?.Invoke(_typedWord.Length, false);
                return;
            }

            var highlightedWord = GlobalRef.I.Words.Where(x => x.GetComponent<WordObject>().GetWord().StartsWith(s)).ToArray();
            if (highlightedWord.Length > 0) {
                AttemptInput(s);
            }
            else {
                if (GlobalRef.I.PrevHighlightedWords.Count > 0) {
                    KeyInput.Instance.ResetText(_typedWord);

                    if (GlobalRef.I.PrevHighlightedWords.Contains(this.gameObject) && _word.StartsWith(_typedWord)) {
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
                GlobalRef.I.PrevHighlightedWords.Add(this.gameObject);
            }

            if (_word.Equals(value))
            {
                WordSpawner.I.ReleaseWord(_word);
                wordCompleted?.Invoke();
                Reset();
                Setup();
            }
        }
        
        private void Reset() {
            GlobalRef.I.PrevHighlightedWords.Clear();
            _typedWord = "";
            KeyInput.Instance.ResetText();
            WordMatched?.Invoke(_typedWord.Length, false);
        }

        public string GetWord() {
            return _word;
        }

        public bool IsHighlighted() {
            return GlobalRef.I.PrevHighlightedWords.Contains(this.gameObject);
        }
        
    }
}