using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Words.Modifier
{
    public class SwapTextModifier : ITextModifier
    {
        public string Modify(string text)
        {
            char[] chars = text.ToCharArray();

            // swap two adjacent char
            int idx = Random.Range(0, chars.Length - 1);
            char temp = chars[idx];
            chars[idx] = chars[idx + 1];
            chars[idx + 1] = temp;

            return new string(chars);
        }
    }
}
