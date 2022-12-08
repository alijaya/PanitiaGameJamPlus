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

        private Dictionary<WordObject2, bool> _matched = new();

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
        }

        public bool IsWordExist(string word)
        {
            return _wordList.Any(wo => wo.Text == word);
        }

        public bool IsFirstCharUnique(string word)
        {
            return _wordList.All(wo => wo.Text[0] != word[0]);
        }

        public void NotifyMatch(WordObject2 wordObject, bool matched)
        {
            _matched[wordObject] = matched;
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
            // only handle letter, number and symbol
            if (!(char.IsLetterOrDigit(ch) || char.IsSymbol(ch) || char.IsPunctuation(ch))) return;

            // cleanup, just sanity check XD
            LastCompletedWord = null;
            _matched.Clear();

            OnTextInput.Invoke(ch); // let WordObject handle input, hopefully calling NotifyMatch or NotifyComplete

            // there is Completed Word
            if (LastCompletedWord != null)
            {
                HandleResetInput(); // Notify Reset Input, Let each WordObject handle itself
            } else
            {
                var matched = _matched.Where(kv => kv.Value);
                var unmatched = _matched.Where(kv => !kv.Value);

                // there's something matched
                if (matched.Count() > 0)
                {
                    // manually forget the unmatched one

                    if (matched.Count() == 1)
                    {
                        State = WordTrackerState.Unique;
                    } else
                    {
                        State = WordTrackerState.Several;
                    }
                } else
                {
                    // nothing matched, so keep the unmatched one, the player make some mistake

                    if (IsStateNone)
                    {
                        State = WordTrackerState.NoneError;
                    } else if (IsStateSeveral)
                    {
                        State = WordTrackerState.SeveralError;
                    } else if (IsStateUnique)
                    {
                        State = WordTrackerState.UniqueError;
                    }
                }
            }

            // cleanup again, we do it before after, to make sure
            LastCompletedWord = null;
            _matched.Clear();

        }

        public void HandleResetInput()
        {
            State = WordTrackerState.None;
        }
    }
}