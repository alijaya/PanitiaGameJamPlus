using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Core.Words.Modifier
{
    public class PalindromTextModifier : ITextModifier
    {
        public string Modify(string text)
        {
            return text + new string(text.Reverse().ToArray());
        }
    }
}
