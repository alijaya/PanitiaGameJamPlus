using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Core.Words
{
    [DisallowMultipleComponent]
    public class WordObject : MonoBehaviour
    {
        public static string InvalidWord = "aliganteng";
        public static float RandomMin = 1;
        public static float RandomMax = 10;

        public enum WordObjectState
        {
            None,
            Disabled,
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
            Destroy,
        }

        [SerializeField] private bool _receiveInput = true;
        public bool ReceiveInput
        {
            get
            {
                return _receiveInput;
            }
            set
            {
                if (_receiveInput != value)
                {
                    _receiveInput = value;
                    UpdateRegistration();
                }
            }
        }

        public UnityEvent<WordObjectState> OnStateChanged;

        [FoldoutGroup("Text Generation")]
        [InlineButton("GenerateTextRandomDifficulty", "Generate")]
        [SerializeReference]
        public Generator.ITextGenerator TextGenerator;

        [FoldoutGroup("Text Generation")]
        [SerializeReference]
        public List<Modifier.ITextModifier> TextModifiers = new();


        [FoldoutGroup("Word Complete")]
        public CompleteAction WhenComplete = CompleteAction.Regenerate;

        [FoldoutGroup("Word Complete")]
        public UnityEvent OnWordImmediateCompleted;

        [FoldoutGroup("Word Complete")]
        public UniTaskFunc CompleteCheck;

        [FoldoutGroup("Word Complete")]
        public UnityEvent OnWordCompleted;

        public TMP_Text text;

        private string _text = "";
        public string Text {
            get
            {
                return _text;
            }
            private set
            {
                if (_text != value)
                {
                    _text = value;
                    UpdateRegistration();
                }
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
                if (_position != value)
                {
                    _position = value;
                    UpdateRegistration();
                }
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

        private bool _registered = false;
        private CancellationTokenSource completeCheckCancel;

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
            // invoke cancel the complete check
            completeCheckCancel?.Cancel();
        }

        #endregion

        public void UpdateRegistration()
        {
            if (!Application.isPlaying) return;
            if (this && isActiveAndEnabled && ReceiveInput && Position < Text.Length)
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
            UpdateState();
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

            var curText = "";
            if (difficulty <= 0) curText = TextGenerator?.Generate() ?? InvalidWord;
            else curText = TextGenerator?.Generate(difficulty) ?? InvalidWord;

            foreach (var modifier in TextModifiers)
            {
                curText = modifier.Modify(curText);
            }

            Text = curText;
            text.text = Text;
        }

        private void SkipWhiteSpace()
        {
            while (_position < Text.Length && char.IsWhiteSpace(Text[Position]))
            {
                _position++;
            }
            if (Position != _position) Position = _position;
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

                if (LastMatched)
                {
                    if (completeCheckCancel == null) completeCheckCancel = new CancellationTokenSource();

                    WordTracker.I.NotifyMatch(this);
                    Position++;

                    // skip after
                    SkipWhiteSpace();

                    if (Position == Text.Length)
                    {
                        WordComplete().Forget();
                    }
                }
            }
        }

        private async UniTask WordComplete()
        {
            OnWordImmediateCompleted.Invoke();
            WordTracker.I.NotifyComplete(this); // this is immediate

            var success = true;
            // wait for async, it could be cancelled tho
            // for example the text dissappear before the chef reach it
            if (CompleteCheck.target != null)
            {
                success = ! await CompleteCheck.Invoke(completeCheckCancel.Token).SuppressCancellationThrow();
            }
            completeCheckCancel.Dispose();
            completeCheckCancel = null;

            // if somehow destroyed, just break
            if (this == null) return;

            // do stuff if it's really executed
            // only execute when really success
            if (success) OnWordCompleted.Invoke();

            if (WhenComplete == CompleteAction.Regenerate)
            {
                Setup();
            } else if (WhenComplete == CompleteAction.Reset)
            {
                ResetState();
            }
            else if (WhenComplete == CompleteAction.Destroy)
            {
                Destroy(gameObject);
            }
        }

        public void ResetState()
        {
            Position = 0;
            LastMatched = false;
            UpdateState();
        }

        public void HandleWordTrackerState(WordTracker.WordTrackerState wordTrackerState)
        {
            // will reset if the global is none
            if (WordTracker.I.IsStateNone)
            {
                ResetState();
                return;
            }

            // will reset if global match something else and this is doesn't match anything
            if (!WordTracker.I.IsStateError && !LastMatched)
            {
                ResetState();
                return;
            }

            UpdateState();
        }

        public void UpdateState()
        {
            if (!ReceiveInput)
            {
                State = WordObjectState.Disabled;
                return;
            } else if (Position == 0)
            {
                if (WordTracker.I.IsStateNone)
                {
                    State = WordObjectState.None;
                }
                else
                {
                    State = WordObjectState.OthersMatched;
                }
            } else if (Position >= Text.Length)
            {
                State = WordObjectState.Completed;
            } else if (!LastMatched && WordTracker.I.IsStateError)
            {
                State = WordObjectState.Unmatched;
            } else
            {
                State = WordObjectState.Matched;
            }
        }


        #region static util

        public static (UniTask, WordObject) SpawnConstantAsync(string text, Transform parent, CancellationToken ct = default)
        {
            var result = GameObject.Instantiate(GlobalRef.I.WordObjectPrefab, parent);
            result.TextGenerator = new Generator.ConstantTextGenerator()
            {
                Text = text
            };

            async UniTask task()
            {
                await result.OnWordCompleted.ToUniTask(ct).SuppressCancellationThrow();
                // suppress cancellation so will always destroy this
                if (result) Destroy(result.gameObject);
            }

            return (task(), result);
        }

        public static WordObject SpawnConstant(string text, Transform parent)
        {
            var result = GameObject.Instantiate(GlobalRef.I.WordObjectPrefab, parent);
            result.TextGenerator = new Generator.ConstantTextGenerator()
            {
                Text = text
            };
            return result;
        }

        #endregion
    }
}