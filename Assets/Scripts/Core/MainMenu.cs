using RS.Typing.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;

public class MainMenu : MonoBehaviour {
    public SceneReference gameplayScene;

    public void OnStartTyped()
    {
        SceneManager.LoadScene(gameplayScene.Name);
    }
}
