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

        private List<string> _allWordBank;
        private List<string> _easyWordBank;
        private List<string> _normalWordBank;
        private List<string> _hardWordBank;

        private HashSet<string> _usedWords;

        protected override void SingletonAwakened() {
            base.SingletonAwakened();
            _allWordBank = wordsFile.text.ToLower().Replace("\r", "").Split("\n").Where(w => !String.IsNullOrEmpty(w) && w.Length >= 3).ToList();

            // add all possible 1 char
            for (var ch = 'a'; ch <= 'z'; ch++)
            {
                _allWordBank.Add(ch.ToString());
            }

            // add all possible 2 char
            for (var ch1 = 'a'; ch1 <= 'z'; ch1++)
            {
                for (var ch2 = 'a'; ch2 <= 'z'; ch2++)
                {
                    _allWordBank.Add(ch1 + "" + ch2);
                }
            }

            _easyWordBank = _allWordBank.Where(word => word.Length <= easyThreshold).ToList();
            _normalWordBank = _allWordBank.Where(word => word.Length > easyThreshold && word.Length <= normalThreshold).ToList();
            _hardWordBank = _allWordBank.Where(word => word.Length > normalThreshold && word.Length <= hardThreshold).ToList();
            _usedWords = new HashSet<string>();
        }

        public List<string> GetBank(WordDifficulty difficulty)
        {
            switch (difficulty)
            {
                case WordDifficulty.Easy:
                    return _easyWordBank;
                case WordDifficulty.Hard:
                    return _hardWordBank;
                case WordDifficulty.Normal:
                default:
                    return _normalWordBank;
            }
        }

        public string GetRandomWord(WordDifficulty difficulty, bool uniqueStart = false) {
            var random = new System.Random();

            var bank = GetBank(difficulty).Except(_usedWords); // ga boleh yang udah dipake

            if (uniqueStart) bank = bank.Where(w => _usedWords.All(uw => !w.StartsWith(uw[0]))); // ga boleh yang kata depannya udah ada

            if (bank.Count() > 0)
            {
                var word = bank.GetRandom();
                _usedWords.Add(word);

                return word;
            }
            else
            {
                return null; // ga ada yang mungkin, sad :(
            }
        }

        public void ReleaseWord(string word) // sudah pake dibalikin ya
        {
            _usedWords.Remove(word);
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

    public enum WordDifficulty {
        Easy,
        Normal,
        Hard
    }
}