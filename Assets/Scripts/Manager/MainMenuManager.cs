using RS.Typing.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;

public class MainMenuManager : MonoBehaviour {
    public SceneReference gameplayScene;

    private void Start()
    {
        GlobalRef.I.PlayBGM_MainMenu();
    }

    public void OnStartTyped()
    {
        SceneManager.LoadScene(gameplayScene.Name);
    }
}
