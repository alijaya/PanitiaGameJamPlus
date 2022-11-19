using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace RS.Typing.Core {
    public class WordObject : MonoBehaviour {
        public string testString;
        public static event EventHandler<bool> WordMatched;
        private static WordObject _lockedWordObject;
        
        [SerializeField] private TMP_Text text;
        [SerializeField] private UnityEvent wordDestroyed;

        private IDisposable _inputCall;

        private string _word;

        private void Start() {
            Setup(testString);
        }

        public void Setup(string word) {
            _word = word;
            text.text = _word;

            _inputCall = InputSystem.onAnyButtonPress.Call(Action);
        }

        private void Action(InputControl ctrl) {
            if (_lockedWordObject != null && _lockedWordObject != this) return;
            if (ctrl.name.Length == 1) AttemptInput(ctrl.name);
        }

        private void AttemptInput(string c) {
            if (_word == "") return;
            if (!_word.StartsWith(c)) {
                Error();
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
                _lockedWordObject = null;
                _inputCall.Dispose();
                wordDestroyed?.Invoke();
                
                Setup(testString);
            }
        }

        private void Error() {
            WordMatched?.Invoke(this, false);
        }
        
    }
}