using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Words.Modifier
{
    public class ChangeCaseTextModifier : ITextModifier
    {
        public enum ChangeCaseType
        {
            LowerCase,
            UpperCase,
            RandomCase
        };

        public ChangeCaseType CaseType = ChangeCaseType.UpperCase;

        public string Modify(string text)
        {
            switch (CaseType)
            {
                case ChangeCaseType.LowerCase: return text.ToLower();
                case ChangeCaseType.UpperCase: return text.ToUpper();
                case ChangeCaseType.RandomCase:
                    var result = "";
                    foreach (char c in text)
                    {
                        int randomNumber = UnityEngine.Random.Range(0, 2);
                        if (randomNumber == 0)
                        {
                            result += char.ToUpper(c);
                        }
                        else
                        {
                            result += char.ToLower(c);
                        }
                    }

                    return result;
                default: return text;
            }
        }
    }
}
