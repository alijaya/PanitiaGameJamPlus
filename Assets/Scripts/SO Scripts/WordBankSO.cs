using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WordBankSO : ScriptableObject
{
    public List<string> Words;

    public string GetRandom()
    {
        return Words.GetRandom();
    }

    public string GetRandomByDifficulty(float difficulty)
    {
        var length = Mathf.RoundToInt(difficulty > 0 ? difficulty : 4);
        return Words.Where(w => w.Count(c => c != ' ') == length).GetRandom();
    }
}
