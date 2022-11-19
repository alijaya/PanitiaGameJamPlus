using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
namespace RS.Typing.Core {
    public class WordObject : MonoBehaviour {
        public string testString;
        public static event EventHandler<bool> WordMatched;
        private static WordObject _lockedWordObject;
        
        [SerializeField] private TMP_Text text;
        [SerializeField] private UnityEvent wordCompleted;

        private IDisposable _inputCall;

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

        private void Action(string s) {
            Debug.Log($"from: {gameObject}, debug:{_lockedWordObject}");
            if (_lockedWordObject == null) {
                if (s.Length == 1) AttemptInput(s);
            } else if (_lockedWordObject == this) {
                if (s.Length == 1) AttemptInput(s);    
            }
        }

        private void AttemptInput(string c) {
            if (_word == "") return;
            if (!_word.StartsWith(c)) {
                if (_lockedWordObject) Error();
                return;
            }
            
            _lockedWordObject = this;
            _word = _word.Remove(0, 1);
            text.text = _word;

            WordMatched?.Invoke(_word == "" ? null: this, true);
            CheckEmpty();
        }
        private void CheckEmpty() {
            if (_word == "") {
                wordCompleted?.Invoke();
                Setup(testString);
                _lockedWordObject = null;
            }
        }

        private void Error() {
            WordMatched?.Invoke(this, false);
        }
        
    }
}