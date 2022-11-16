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
                _instance = Resources.Load(typeof(T).Name) as T;
            }
            return _instance;
        }
    }
}
