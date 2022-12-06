using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Words
{
    public class DictionaryTextGenerator : ITextGenerator
    {
        public WordDifficulty Difficulty = WordDifficulty.Normal;
        public string Generate(float difficulty = 0)
        {
            return WordSpawner.I.GetRandomWord(Difficulty, true);
        }
    }
}
