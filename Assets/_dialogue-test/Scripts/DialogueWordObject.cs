using System.Collections;
using System.Collections.Generic;
using Core.Words;
using UnityEngine;

public class DialogueWordObject : WordObject
{
    [SerializeField] private TypeStoryManager manager;

    // public override void Start()
    // {
    //     typo = manager.currentText.ToLower();
    //     base.Start();
    // }

    // public override void Setup()
    // {
    //     _word = typo;
    //     text.text = _word;
    // }

    // TODO: ummm ini apa?
    //public override void WordComplete()
    //{
    //    typo = manager.currentText.ToLower();
    //    Reset();
    //    Setup();
            
    //    KeyInput.Instance.SetEnable(true);
    //}
}
