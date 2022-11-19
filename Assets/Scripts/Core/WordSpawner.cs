using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RS.Typing.Core {
    public class WordSpawner : SingletonMB<WordSpawner> {
        [SerializeField] private TextAsset wordsFile;
        [SerializeField] private int easyThreshold;
        [SerializeField] private int normalThreshold;
        [SerializeField] private int hardThreshold;

        private List<string> _easyWordBank;
        private List<string> _normalWordBank;
        private List<string> _hardWordBank;

        protected override void SingletonAwakened() {
            base.SingletonAwakened();
            _easyWordBank = wordsFile.text.Split("\n").Where(word => word.Length < easyThreshold).ToList();
            _normalWordBank = wordsFile.text.Split("\n").Where(word => word.Length > easyThreshold && word.Length < normalThreshold).ToList();
            _hardWordBank = wordsFile.text.Split("\n").Where(word => word.Length > normalThreshold && word.Length < hardThreshold).ToList();
        }

        public string GetRandomWord(WordDifficult difficult) {
            var random = new System.Random();
            var word = "";
            switch (difficult)
            {
                case WordDifficult.Easy: 
                    word = _easyWordBank.GetRandom();
                    break;
                case WordDifficult.Hard:
                    word = _hardWordBank.GetRandom();
                    break;
                case WordDifficult.Normal:
                default:
                    word = _normalWordBank.GetRandom();
                    break;
            }

            return word;
        }

        private IEnumerable<string> GetRandomWords(int amount) {
            var words = new List<string>();
            var random = new System.Random();

            while (words.Count < amount) {
                var startingChars = words.Select(s => s[0]);
                var word = _normalWordBank.ElementAt(random.Next(0, _normalWordBank.Count));
                
                if (words.Contains(word)) continue;
                if (startingChars.Any(c => c == word[0])) continue;
                
                words.Add(word);
            }
            return words;
        }

    }

    public enum WordDifficult {
        Easy,
        Normal,
        Hard
    }
}