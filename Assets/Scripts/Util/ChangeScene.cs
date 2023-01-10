using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eflatun.SceneReference;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public SceneReference scene;

    public void GoToScene()
    {
        SceneManager.LoadScene(scene.Name);
    }

}
