using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Words.Generator
{
    public class ConstantTextGenerator : ITextGenerator
    {
        public string Text;

        public string Generate()
        {
            return Text;
        }

        public string Generate(float difficulty)
        {
            return Generate();
        }
    }
}
