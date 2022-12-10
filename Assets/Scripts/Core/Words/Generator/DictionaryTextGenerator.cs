using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Words.Generator
{
    public class DictionaryTextGenerator : ITextGenerator
    {
        public WordDifficulty Difficulty = WordDifficulty.Normal;

        public string Generate()
        {
            return WordSpawner.I.GetRandomWord(Difficulty, true);
        }

        public string Generate(float difficulty)
        {
            return Generate();
        }
    }
}
