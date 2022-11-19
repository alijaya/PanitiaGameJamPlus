using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
namespace RS.Typing.Core {
    public class WordObject : MonoBehaviour {
        public string testString;
        public static event EventHandler<bool> WordMatched;

        [SerializeField] private TMP_Text text;
        [SerializeField] private UnityEvent wordCompleted;

        private IDisposable _inputCall;

        private static KeyValuePair<WordObject, string> _previous;

        private string _word;

        private void Start() {
            Setup(testString);
        }

        public void Setup(string word) {
            _word = word;
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
            
            AttemptInput(s, wordObject);
        }

        private void AttemptInput(string c, bool isNull) {
            if (!_word.StartsWith(c)) {
                if (isNull) Error();
                return;
            }

            KeyInput.Instance.lockedWord = this;
            _word = _word.Remove(0, 1);
            text.text = _word;

            WordMatched?.Invoke(_word == "" ? null: this, true);
            if (CheckEmpty()) {
                KeyInput.Instance.lockedWord = null;
            }
        }
        private bool CheckEmpty() {
            if (_word != "") return false;
            wordCompleted?.Invoke();
            Setup(testString);
            return true;
        }

        private void Error() {
            WordMatched?.Invoke(this, false);
        }
        
    }
}