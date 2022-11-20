using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RS.Typing.Core {
    public class WordObject : MonoBehaviour {
        public WordDifficulty difficulty = WordDifficulty.Normal;
        public UnityEvent<int, bool> WordMatched;

        [SerializeField] private string typo;
        [SerializeField] private TMP_Text text;
        public UnityEvent wordCompleted;
        public UnityEvent<UnityEvent> wordCompletedDelegate;

        [SerializeField] private Transform objectiveTransform;
        private ChefTasks _chef;
        
        private string _word;
        private static string _typedWord;

        private void Awake() {
            _chef = FindObjectOfType<ChefTasks>();
            if (_chef) wordCompletedDelegate.AddListener(_chef.AddTask);
            wordCompletedDelegate.AddListener(Call);
            wordCompleted.AddListener(WordComplete);
        }

        private void Call(UnityEvent arg0) {
            if (_chef) _chef.MoveTo(objectiveTransform);
        }

        private void Start() {
            GlobalRef.I.Words.Add(this.gameObject);
            Setup();
        }

        private void Setup() {
            _word = string.IsNullOrEmpty(typo) ? WordSpawner.I.GetRandomWord(difficulty, true) : typo;
            text.text = _word;
        }

        private void OnDestroy()
        {
            GlobalRef.I.Words.Remove(this.gameObject);
            if (_chef) wordCompletedDelegate.RemoveListener(_chef.AddTask);
            wordCompletedDelegate.RemoveListener(Call);
            wordCompleted.RemoveListener(WordComplete);
        }

        private void OnEnable() {
            KeyInput.KeyDown += Action;
        }

        private void OnDisable() {
            KeyInput.KeyDown -= Action;
        }

        private void Action(string s) { // ini kok kayak fungsi global, tapi ada di setiap instance? butuh refactor kah?
            if (string.IsNullOrEmpty(s)) {
                GlobalRef.I.PrevHighlightedWords.Clear();
                _typedWord = "";
                WordMatched.Invoke(_typedWord.Length, false);
                return;
            }

            var highlightedWord = GlobalRef.I.Words.Where(x => x.GetComponent<WordObject>().GetWord().StartsWith(s)).ToArray();
            if (highlightedWord.Length > 0) { // kalau ada yang depannya sama dengan yang kita tulis
                AttemptInput(s);
            }
            else { // kalau ga ada yang sama sekali
                if (GlobalRef.I.PrevHighlightedWords.Count > 0) { // kalau sebelumnya udah ada yang di highlight, artinya kita salah ngetik di tengah2
                    KeyInput.Instance.ResetText(_typedWord); // kita undo text yang diketik, seolah2 ga ngetik apa2

                    if (GlobalRef.I.PrevHighlightedWords.Contains(this.gameObject) && _word.StartsWith(_typedWord)) { // kalau object ini salah satunya
                        WordMatched.Invoke(_typedWord.Length, false); // trigger event match tapi salah
                    }
                }
                else { // kalau sebelumnya ga ada yang di highlight, artinya dari awal dah salah ketik, reset aja jadi kosongan
                    KeyInput.Instance.ResetText();
                }
            }
        }

        private void AttemptInput(string value) {
            if (_word.StartsWith(value)) { // kalau depannya sama
                _typedWord = value; // yes simpen aja tulisannya, karena valid
                WordMatched.Invoke(_typedWord.Length, true); // trigger match
                if (!GlobalRef.I.PrevHighlightedWords.Contains(this.gameObject)) // kalau blom ada di list
                {
                    GlobalRef.I.PrevHighlightedWords.Add(this.gameObject); // tambah object ini karena match
                }
            }
            else // kalau di attempt ga bisa, artinya udah pindah ke lain hati highlightnya, ada yang match tapi bukan disini
            {
                GlobalRef.I.PrevHighlightedWords.Remove(this.gameObject);
                WordMatched.Invoke(0, false);
            }

            if (_word.Equals(value)) {
                KeyInput.Instance.SetEnable(false);
                WordSpawner.I.ReleaseWord(_word); // balikin kata2nya
                wordCompletedDelegate?.Invoke(wordCompleted);
            }
        }

        private void WordComplete() {
            Reset();
            Setup();
            
            KeyInput.Instance.SetEnable(true);
        }

        public void Reset() {
            GlobalRef.I.PrevHighlightedWords.Clear();
            _typedWord = "";
            KeyInput.Instance.ResetText();
            WordMatched.Invoke(_typedWord.Length, false);
        }

        public string GetWord() {
            return _word;
        }

        public bool IsHighlighted() {
            return GlobalRef.I.PrevHighlightedWords.Contains(this.gameObject);
        }
        
    }
}