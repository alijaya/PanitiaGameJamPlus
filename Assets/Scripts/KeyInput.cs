using System;
using TMPro;
using UnityEngine;

public class KeyInput : Singleton<KeyInput> {
    [SerializeField]private TMP_InputField inputField;
    public static event Action<string> KeyDown;

    public void DebugShow(string value) {
        if (Input.anyKeyDown) {
            if (value !="") KeyDown?.Invoke(value);  
        }
        inputField.text = "";
    }
}