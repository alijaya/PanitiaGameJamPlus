using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Words.Generator
{
    public class WordBankTextGenerator : ITextGenerator
    {
        public WordBankSO WordBank;

        public string Generate()
        {
            return WordBank.GetRandom();
        }

        public string Generate(float difficulty)
        {
            return WordBank.GetRandomByDifficulty(difficulty);
        }
    }
}
