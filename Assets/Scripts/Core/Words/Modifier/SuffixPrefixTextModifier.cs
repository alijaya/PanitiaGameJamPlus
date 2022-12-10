using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Words.Modifier
{
    public class SuffixPrefixTextModifier : ITextModifier
    {
        public string Suffix = "";
        public string Prefix = "";

        public string Modify(string text)
        {
            return Suffix + text + Prefix;
        }
    }
}
