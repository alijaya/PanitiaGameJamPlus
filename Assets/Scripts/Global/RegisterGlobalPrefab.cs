using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// prefab yang ada disini bakal ke load otomatis di semua scene, tanpa perlu masukin manual satu2 ke scenenya
[CreateAssetMenu]
public class RegisterGlobalPrefab : SingletonSO<RegisterGlobalPrefab>
{
    public List<GameObject> globalPrefabs;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntime()
    {
        var autoRuntimeInit = I;
        foreach (var prefab in I.globalPrefabs)
        {
            var go = Instantiate(prefab);
            DontDestroyOnLoad(go);
        }
    }
}
