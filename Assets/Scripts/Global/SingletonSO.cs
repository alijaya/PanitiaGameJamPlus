using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// musti buat SO nya di folder Resources pakai nama yang sama dengan Class nya
public abstract class SingletonSO<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance;
    public static T I
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoadOrCreateInstance<T>();
            }
            return _instance;
        }
    }

    public static TInstance LoadOrCreateInstance<TInstance>() where TInstance : ScriptableObject
    {
        var ins = Resources.Load(typeof(TInstance).Name) as TInstance;

        if (ins == null)
        {
#if !UNITY_EDITOR
            // should throw error?
            Debug.LogWarning($"Singleton {typeof(TInstance).Name} not found");
#else
            // actually create it

            ins = CreateInstance<TInstance>();

            UnityEditor.AssetDatabase.CreateAsset(ins, $"Assets/Resources/{typeof(TInstance).Name}.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        return ins;
    }
}
