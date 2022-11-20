using RS.Typing.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;

public class MainMenuManager : MonoBehaviour {
    public SceneReference gameplayScene;

    public void OnStartTyped()
    {
        SceneManager.LoadScene(gameplayScene.Name);
    }
}
