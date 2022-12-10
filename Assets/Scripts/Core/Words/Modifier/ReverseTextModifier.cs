using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Core.Words.Modifier
{
    public class ReverseTextModifier : ITextModifier
    {
        public string Modify(string text)
        {
            return new string(text.Reverse().ToArray());
        }
    }
}
