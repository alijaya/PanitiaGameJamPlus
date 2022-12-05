using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class KeyInput : SingletonMB<KeyInput> {

    public string CurrentText { get; private set; } = "";
    public event Action<string> CurrentTextChanged;

    protected override void SingletonAwakened()
    {
        SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        OnSceneLoaded();
    }

    protected override void SingletonDestroyed()
    {
        SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
    }

    private void OnEnable() {
        Keyboard.current.onTextInput += TypeChar;
    }

    private void OnDisable() {
        Keyboard.current.onTextInput -= TypeChar;
    }

    private void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
        OnSceneLoaded();
    }

    private void OnSceneLoaded()
    {
        this.enabled = true;
        ResetText();
    }

    public void Update() {
        if (Keyboard.current.backspaceKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ResetText();
        }
    }

    public void SetText(string value = "")
    {
        if (CurrentText != value)
        {
            CurrentText = value;
            //Debug.Log(CurrentText);
            CurrentTextChanged?.Invoke(CurrentText);
        }
    }

    public void TypeChar(char value)
    {
        SetText(CurrentText + value);
    }

    public void TypeString(string value)
    {
        SetText(CurrentText + value);
    }

    public void ResetText() {
        SetText("");
    }

    public void DeleteChar(int count = 1)
    {
        SetText(CurrentText.Remove(CurrentText.Length - count));
    }
}