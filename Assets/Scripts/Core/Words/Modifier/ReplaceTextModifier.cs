using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Words.Modifier
{
    public class ReplaceTextModifier : ITextModifier
    {
        public string FromText = "";
        public string ToText = "";

        public string Modify(string text)
        {
            return text.Replace(FromText, ToText);
        }
    }
}
