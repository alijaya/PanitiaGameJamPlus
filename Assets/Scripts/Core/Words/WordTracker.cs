using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Core.Words
{
    public class WordTracker : SingletonMB<WordTracker>
    {
        public enum WordTrackerState
        {
            None,
            Several,
            Unique,
            NoneError,
            SeveralError,
            UniqueError,
        };

        private HashSet<WordObject2> _wordList = new();

        private HashSet<WordObject2> _matched = new();

        public WordObject2 LastCompletedWord { get; private set; }

        private WordTrackerState _state = WordTrackerState.None;
        public WordTrackerState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                OnStateChanged.Invoke(value);
            }
        }

        public bool IsStateNone
        {
            get
            {
                return State == WordTrackerState.None || State == WordTrackerState.NoneError;
            }
        }

        public bool IsStateSeveral
        {
            get
            {
                return State == WordTrackerState.Several || State == WordTrackerState.SeveralError;
            }
        }

        public bool IsStateUnique
        {
            get

            {
                return State == WordTrackerState.Unique || State == WordTrackerState.UniqueError;
            }
        }


        public bool IsStateError
        {
            get
            {
                return State == WordTrackerState.NoneError || State == WordTrackerState.SeveralError || State == WordTrackerState.UniqueError;
            }
        }

        public UnityEvent<WordTrackerState> OnStateChanged;
        public UnityEvent<char> OnTextInput;
        public UnityEvent OnResetInput;

        private bool isInputText = false;

        private void OnEnable()
        {
            Keyboard.current.onTextInput += HandleTextInput;
        }
        private void OnDisable()
        {
            Keyboard.current.onTextInput -= HandleTextInput;
        }

        private void Update()
        {
            if (Keyboard.current.backspaceKey.wasPressedThisFrame) HandleResetInput();
        }

        public void RegisterWord(WordObject2 word)
        {
            _wordList.Add(word);
        }

        public void UnregisterWord(WordObject2 word)
        {
            _wordList.Remove(word);

            // this is prone to bugs
            // only update state if it's not in input sequence
            if (!isInputText)
            {
                // remove word if matched
                _matched.Remove(word);
                if (LastCompletedWord == word) LastCompletedWord = null;
                UpdateState();
            }
        }

        public bool IsWordExist(string word)
        {
            return _wordList.Any(wo => wo.Text == word);
        }

        public bool IsFirstCharUnique(string word)
        {
            return _wordList.All(wo => wo.Text[0] != word[0]);
        }

        public void NotifyMatch(WordObject2 wordObject)
        {
            _matched.Add(wordObject);
        }

        public void NotifyComplete(WordObject2 wordObject)
        {
            if (LastCompletedWord == null)
            {
                LastCompletedWord = wordObject;
            }
        }

        public void HandleTextInput(char ch)
        {
            isInputText = true; // so we could know it's caused by input or not

            // only handle letter, number and symbol
            if (!(char.IsLetterOrDigit(ch) || char.IsSymbol(ch) || char.IsPunctuation(ch))) return;

            // cleanup
            LastCompletedWord = null;
            _matched.Clear();

            OnTextInput.Invoke(ch); // let WordObject handle input, hopefully calling NotifyMatch or NotifyComplete

            // there is Completed Word
            if (LastCompletedWord != null)
            {
                HandleResetInput(); // Notify Reset Input, Let each WordObject handle itself
            } else
            {
                UpdateState();
            }

            // cleanup again, we do it before after, to make sure
            //LastCompletedWord = null;
            //_matched.Clear();

            isInputText = false;
        }

        public void UpdateState()
        {
            // there's something matched
            if (_matched.Count() > 0)
            {
                // manually forget the unmatched one

                if (_matched.Count() == 1)
                {
                    State = WordTrackerState.Unique;
                }
                else
                {
                    State = WordTrackerState.Several;
                }
            }
            else if (isInputText) // if this is triggered from input
            {
                // nothing matched, so keep the unmatched one, the player make some mistake

                if (IsStateNone)
                {
                    State = WordTrackerState.NoneError;
                }
                else if (IsStateSeveral)
                {
                    State = WordTrackerState.SeveralError;
                }
                else if (IsStateUnique)
                {
                    State = WordTrackerState.UniqueError;
                }
            } else // if it's not triggered from input and nothing matches, then it's not error, just set to none
            {
                State = WordTrackerState.None;
            }
        }

        public void HandleResetInput()
        {
            _matched.Clear();
            LastCompletedWord = null;
            State = WordTrackerState.None;
        }
    }
}