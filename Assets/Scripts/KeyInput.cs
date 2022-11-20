using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class KeyInput : Singleton<KeyInput> {
    [SerializeField]private TMP_InputField inputField;
    public static event Action<string> KeyDown;

    private bool _enabled = true;

    protected override void Awake() {
        base.Awake();
        SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= SceneManagerOnsceneLoaded;
    }

    private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1) {
        EventSystem.current.SetSelectedGameObject(gameObject);
        ResetText();
        _enabled = true;
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }

    public void DebugShow(string value) {
        if (Input.anyKeyDown && _enabled) {
            KeyDown?.Invoke(value?.ToLower());
        }
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape)) {
            ResetText();
            DebugShow("");
        }
    }

    public void ResetText(string value = "") {
        inputField.text = value;
    }

    public void SetEnable(bool value) {
        _enabled = value;
    }
}