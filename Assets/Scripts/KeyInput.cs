using System;
using TMPro;
using UnityEngine;

public class KeyInput : Singleton<KeyInput> {
    [SerializeField]private TMP_InputField inputField;
    public static event Action<string> KeyDown;

    public void DebugShow(string value) {
        if (Input.anyKeyDown) {
            KeyDown?.Invoke(value?.ToLower());
        }
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape)) {
            ResetText();
        }
    }

    public void ResetText(string value = "") {
        inputField.text = value;
    }
}