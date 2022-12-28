using System;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class QuitUtil
{
    public static bool isQuitting;
    public static bool isLoaded;

    // things get destroyed when Application is quitting
    // activeScene isLoaded == false when on starting loading it before Start called
    // so when in Awake or OnEnable, it's still false
    // only applied when calling SceneManager.LoadScene, when starting from Editor
    // it's directly to true
    // and isLoaded == false when destroying stuff, before fully unloaded
    // so it's destroying when unloading stuff, isLoaded == false and it's not
    // fully unloaded yet
    public static bool isDestroying
    {
        get
        {
            return isQuitting || (!SceneManager.GetActiveScene().isLoaded && isLoaded);
        }
    }

#if UNITY_EDITOR
    [InitializeOnEnterPlayMode]
    static void EnterPlayMode(EnterPlayModeOptions options)
    {
        isQuitting = false;
        isLoaded = true;
    }
#endif

    [RuntimeInitializeOnLoadMethod]
    static void RunOnStart()
    {
        isQuitting = false;
        isLoaded = true;
        Application.quitting += Quit;
        SceneManager.activeSceneChanged += ActiveSceneChanged;
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnloaded;
    }

    private static void SceneUnloaded(Scene scene)
    {
        // set isUnloaded to true if Active Scene is 
        isLoaded = SceneManager.GetActiveScene().isLoaded;
        //Debug.Log(scene.name + ": Unloaded" + GetActiveScene());
    }

    private static void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isLoaded = SceneManager.GetActiveScene().isLoaded;
        //Debug.Log(scene.name + ": Loaded, " + mode.ToString() + GetActiveScene());
    }

    private static void ActiveSceneChanged(Scene scene, Scene nextScene)
    {
        //Debug.Log(scene.name + " > " + nextScene.name + ": Active Scene Changed" + GetActiveScene());
    }

    public static string GetActiveScene()
    {
        var activeScene = SceneManager.GetActiveScene();
        return " (" + activeScene.name + ": " + activeScene.isLoaded + ")";
    }

    static void Quit()
    {
        //Debug.Log("Application Quit" + GetActiveScene());
        isQuitting = true;
        Application.quitting -= Quit;
        SceneManager.activeSceneChanged -= ActiveSceneChanged;
        SceneManager.sceneLoaded -= SceneLoaded;
        SceneManager.sceneUnloaded -= SceneUnloaded;
    }
}