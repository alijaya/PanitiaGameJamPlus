using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Words
{
    public interface ITextGenerator
    {
        public abstract string Generate();
        public abstract string Generate(float difficulty);
    }
}
