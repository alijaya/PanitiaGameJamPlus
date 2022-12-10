using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Words.Modifier
{
    public interface ITextModifier
    {
        public abstract string Modify(string text);
    }
}
