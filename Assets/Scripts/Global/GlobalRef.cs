using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using Eflatun.SceneReference;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GlobalRef : SingletonSO<GlobalRef>
{
    public GameObjectValueList Words;
    public GameObjectValueList PrevHighlightedWords;

    public void CleanUpWords()
    {
        Words.Clear();
        PrevHighlightedWords.Clear();
    }
}
