using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RS.Typing.Core { 
    public class WordPrompt : MonoBehaviour {
        private readonly List<WordObject> _wordObjectList = new ();
        private WordObject _focusedWordObject;
        
        private int _position;

        private void OnEnable() {
            WordObject.AnyNewWordObjectGenerated += OnAnyNewWordObjectGenerated;
            WordObject.AnyWordObjectDestroyed += OnAnyWordObjectDestroyed;
            Keyboard.current.onTextInput += OnTextInput;
        }
        private void OnDisable() {
            WordObject.AnyNewWordObjectGenerated -= OnAnyNewWordObjectGenerated;
            WordObject.AnyWordObjectDestroyed -= OnAnyWordObjectDestroyed;
            Keyboard.current.onTextInput -= OnTextInput;
        }

        private void Update() {
            if (Keyboard.current.backspaceKey.wasPressedThisFrame && _focusedWordObject) {
                _focusedWordObject.Reset();
                _focusedWordObject = null;
                _position = 0;
            }
        }

        private void OnAnyNewWordObjectGenerated(object sender, EventArgs e) {
            var wordObject = (WordObject)sender;
            if (_wordObjectList.Contains(wordObject)) return;
            _wordObjectList.Add(wordObject);
        }

        private void OnAnyWordObjectDestroyed(object sender, EventArgs e) {
            var wordObject = (WordObject)sender;
            if (!_wordObjectList.Contains(wordObject)) return;
            _wordObjectList.Remove(wordObject);
        }

        private void OnTextInput(char ch) {
            if (_focusedWordObject == null) {
                _focusedWordObject = _wordObjectList.FirstOrDefault(x => x.Text.StartsWith(ch));
            }

            if (_focusedWordObject == null) {
                // no word started with ch
                return;
            }

            if (_focusedWordObject.Text[_position] == ch) {
                ++_position;
                _focusedWordObject.Highlight(_position, true);
                if (_position == _focusedWordObject.Text.Length) {
                    // Word Complete
                    _focusedWordObject.WordComplete();
                    _focusedWordObject = null;
                    _position = 0;
                }
            }
            else {
                // HighlightError
                _focusedWordObject.Highlight(_position, false);
            }
        }
    }
}