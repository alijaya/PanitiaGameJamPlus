using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Words
{
    public class ConstantTextGenerator : ITextGenerator
    {
        public string Text;

        public string Generate(float difficulty = 0)
        {
            return Text;
        }
    }
}
