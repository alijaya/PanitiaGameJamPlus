using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Words.Generator
{
    public interface ITextGenerator
    {
        public abstract string Generate();
        public abstract string Generate(float difficulty);
    }
}
