using System;
using RS.Typing.Core;
using TMPro;
using UnityEngine;

public class KeyInput : Singleton<KeyInput> {
    [SerializeField]private TMP_InputField inputField;
    public static event Action<string, WordObject> KeyDown;
    public WordObject lockedWord;
    
    public void DebugShow(string value) {
        if (Input.anyKeyDown) {
            if (value !="") KeyDown?.Invoke(value, lockedWord);  
        }
        inputField.text = "";
    }
}