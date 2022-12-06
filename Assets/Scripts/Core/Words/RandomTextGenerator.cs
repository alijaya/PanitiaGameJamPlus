using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Words
{
    public class RandomTextGenerator : ITextGenerator
    {
        public bool IncludeLowerCase = true;
        public bool IncludeUpperCase = false;
        public bool IncludeNumber = false;
        public bool IncludeSymbol = false;

        public static readonly string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        public static readonly string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static readonly string number = "0123456789";
        public static readonly string symbol = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";

        public string Generate(float? difficulty)
        {
            var pool = "";
            if (IncludeLowerCase) pool += lowerCase;
            if (IncludeUpperCase) pool += upperCase;
            if (IncludeNumber) pool += number;
            if (IncludeSymbol) pool += symbol;

            // if all false, force to lowerCase only
            if (pool.Length == 0) pool = lowerCase;

            var result = "";
            var length = Mathf.RoundToInt(difficulty ?? 4);
            for (var i = 0; i < length; i++)
            {
                result += pool.GetRandom();
            }
            return result;
        }
    }
}
