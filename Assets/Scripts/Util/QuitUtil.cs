using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class QuitUtil
{
    public static bool isQuitting;

    public static bool isDestroying
    {
        get
        {
            return !SceneManager.GetActiveScene().isLoaded || isQuitting;
        }
    }

#if UNITY_EDITOR
    [InitializeOnEnterPlayMode]
    static void EnterPlayMode(EnterPlayModeOptions options)
    {
        isQuitting = false;
    }
#endif

    [RuntimeInitializeOnLoadMethod]
    static void RunOnStart()
    {
        isQuitting = false;
        Application.quitting += Quit;
    }

    static void Quit()
    {
        isQuitting = true;
        Application.quitting -= Quit;
    }
}