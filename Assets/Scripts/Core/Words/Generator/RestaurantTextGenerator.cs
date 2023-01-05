using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Words.Generator
{
    public class RestaurantTextGenerator : ITextGenerator
    {
        public string Generate()
        {
            return RestaurantManager.I.defaultGenerator.Generate();
        }

        public string Generate(float difficulty)
        {
            return RestaurantManager.I.defaultGenerator.Generate(difficulty);
        }
    }
}
