using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace Core.Words
{
    [DisallowMultipleComponent]
    public class WordObject2 : SerializedMonoBehaviour
    {
        public static float RandomMin = 1;
        public static float RandomMax = 10;

        public enum WordObjectState
        {
            None,
            Matched,
            Unmatched,
            OthersMatched,
            Completed,
        }

        public enum CompleteAction
        {
            None,
            Reset,
            Regenerate,
        }

        public CompleteAction WhenComplete = CompleteAction.Regenerate;

        [InlineButton("GenerateTextRandomDifficulty", "Generate")]
        public ITextGenerator TextGenerator;

        public UnityEvent OnWordImmediateCompleted;
        public Func<UniTask> CompleteCheck;
        public UnityEvent OnWordCompleted;

        public UnityEvent<WordObjectState> OnStateChanged;

        public TMP_Text text;

        private string _text = "";
        public string Text {
            get
            {
                return _text;
            }
            private set
            {
                _text = value;
                UpdateRegistration();
            }
        }
        private int _position = 0;
        public int Position {
            get
            {
                return _position;
            }
            private set
            {
                _position = value;
                UpdateRegistration();
            }
        }
        public bool LastMatched { get; private set; } = false;

        private WordObjectState _state = WordObjectState.None;
        public WordObjectState State
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

        private bool _receiveInput = true;
        public bool ReceiveInput
        {
            get
            {
                return _receiveInput;
            }
            set
            {
                _receiveInput = value;
                UpdateRegistration();
            }
        }

        private bool _registered = false;

        #region MonobehaviorLifeCycle

        private void Start()
        {
            _registered = false;
            Setup();
        }

        private void OnEnable()
        {
            UpdateRegistration();
        }

        private void OnDisable()
        {
            UpdateRegistration();
        }

        #endregion

        public void UpdateRegistration()
        {
            if (isActiveAndEnabled && ReceiveInput && Position < Text.Length)
            {
                if (_registered) return;
                _registered = true;
                WordTracker.I.RegisterWord(this);
                WordTracker.I.OnTextInput.AddListener(TextInput);
                WordTracker.I.OnStateChanged.AddListener(HandleWordTrackerState);
            } else
            {
                if (!_registered) return;
                _registered = false;
                WordTracker.I.UnregisterWord(this);
                WordTracker.I.OnTextInput.RemoveListener(TextInput);
                WordTracker.I.OnStateChanged.RemoveListener(HandleWordTrackerState);
            }
        }

        protected void Setup()
        {
            ResetState();
            GenerateText();
        }

        public void GenerateTextRandomDifficulty()
        {
            GenerateText();
        }

        public void GenerateText(float difficulty = 0)
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObjects(new UnityEngine.Object[]{this, text}, "Generate Text");
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(text);
#endif

            if (difficulty <= 0) Text = TextGenerator?.Generate() ?? WordSpawner.InvalidWord;
            else Text = TextGenerator?.Generate(difficulty) ?? WordSpawner.InvalidWord;
            text.text = Text;
        }

        private void SkipWhiteSpace()
        {
            while (Position < Text.Length && char.IsWhiteSpace(Text[Position]))
            {
                Position++;
            }
        }

        public void TextInput(char ch)
        {
            // handle depends on global state
            // needs the last state to be none or already progressing
            if (WordTracker.I.IsStateNone || Position > 0)
            {

                // skip before
                SkipWhiteSpace();

                LastMatched = Position >= Text.Length || Text[Position] == ch;
                WordTracker.I.NotifyMatch(this, LastMatched);

                if (LastMatched)
                {
                    Position++;

                    // skip after
                    SkipWhiteSpace();

                    State = WordObjectState.Matched;

                    if (Position == Text.Length)
                    {
                        WordComplete().Forget();
                    }
                }
            }
        }

        private async UniTask WordComplete()
        {
            State = WordObjectState.Completed;
            OnWordImmediateCompleted.Invoke();
            WordTracker.I.NotifyComplete(this); // this is immediate

            // wait for async
            if (CompleteCheck != null)
            {
                await CompleteCheck();
            }

            // do stuff if it's really executed
            OnWordCompleted.Invoke();

            if (WhenComplete == CompleteAction.Regenerate)
            {
                Setup();
            } else if (WhenComplete == CompleteAction.Reset)
            {
                ResetState();
            }
        }

        public void ResetState()
        {
            Position = 0;
            LastMatched = false;
            if (WordTracker.I.IsStateNone)
            {
                State = WordObjectState.None;
            }
            else
            {
                State = WordObjectState.OthersMatched;
            }
        }

        public void HandleWordTrackerState(WordTracker.WordTrackerState wordTrackerState)
        {
            if (WordTracker.I.IsStateNone)
            {
                ResetState();
                return;
            }

            if (!LastMatched) // only care when LastMatched is false
            {
                if (WordTracker.I.IsStateError) // will invoke unmatch if the global doesn't match anything
                {
                    if (Position > 0) // will invoke if we actually in the middle
                    {
                        State = WordObjectState.Unmatched;
                    }
                    else // if not, it means we are in the beginning, we are good, reset again
                    {
                        ResetState();
                    }
                } else
                {
                    // will reset, if currently not matched, and the global matches something else
                    ResetState();
                }
            } // if matched, we don't care about it
        }

    }
}