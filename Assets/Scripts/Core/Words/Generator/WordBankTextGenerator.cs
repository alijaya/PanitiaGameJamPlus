using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Words.Generator
{
    public class WordBankTextGenerator : ITextGenerator
    {
        public static int tryCount = 10;

        public WordBankSO WordBank;

        public string Generate()
        {
            string word;

            for (var i = 0; i < tryCount; i++)
            {
                word = WordBank.GetRandom();
                if (WordTracker.I.IsWordValid(word)) return word;
            }

            // could not do anything
            return null;
        }

        public string Generate(float difficulty)
        {
            string word;

            for (var i = 0; i < tryCount; i++)
            {
                word = WordBank.GetRandomByDifficulty(difficulty);
                if (WordTracker.I.IsWordValid(word)) return word;
            }

            // could not do anything
            return null;
        }
    }
}
