using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RS.Typing.Core { 
    public class WordPrompt : MonoBehaviour {
        private readonly List<WordObjectBase> _wordObjectList = new ();
        private List<WordObjectBase> _focused = new ();

        private void OnEnable() {
            WordObjectBase.AnyNewWordObjectGenerated += OnAnyNewWordObjectGenerated;
            WordObjectBase.AnyWordObjectDestroyed += OnAnyWordObjectDestroyed;
            Keyboard.current.onTextInput += OnTextInput;

            WordObjectBase.OnCompleted = () => {
                ResetAllWordObject();
                _focused.Clear();
            };
        }
        private void OnDisable() {
            WordObjectBase.AnyNewWordObjectGenerated -= OnAnyNewWordObjectGenerated;
            WordObjectBase.AnyWordObjectDestroyed -= OnAnyWordObjectDestroyed;
            Keyboard.current.onTextInput -= OnTextInput;
        }

        private void Update() {
            if (!Keyboard.current.backspaceKey.wasPressedThisFrame) return;
            ResetAllWordObject();
            _focused.Clear();
        }

        private void OnAnyNewWordObjectGenerated(object sender, EventArgs e) {
            var wordObject = (WordObjectBase)sender;
            if (_wordObjectList.Contains(wordObject)) return;
            _wordObjectList.Add(wordObject);
        }

        private void OnAnyWordObjectDestroyed(object sender, EventArgs e) {
            var wordObject = (WordObjectBase)sender;
            if (!_wordObjectList.Contains(wordObject)) return;
            _wordObjectList.Remove(wordObject);
        }

        private void OnTextInput(char ch) {
            if (HandlePassiveWordObjectInput(ch)) return;
            
            if (_focused.Count == 1) {
                _focused[0].TryMatch(ch, true);
                return;
            }
            
            // Getting new matched words
            var matched = _wordObjectList.
                Where(wordObject => wordObject is not WordObjectPassive).
                Where(wordObject => wordObject.TryMatch(ch, false)).ToList();

            if (matched.Count == 1) {
                foreach (var wordObjectBase in _focused.Except(matched)) {
                    wordObjectBase.Reset();
                }
            }
            
            _focused = matched;
        }

        private bool HandlePassiveWordObjectInput(char ch) {
            return _wordObjectList.OfType<WordObjectPassive>().Any(wordObjectBase => wordObjectBase.TryMatch(ch, false));
        }

        private void ResetAllWordObject() {
            foreach (var wordObject in _focused) {
                wordObject.Reset();
            }
        }
    }
}