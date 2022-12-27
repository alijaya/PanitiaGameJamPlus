using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SingletonMB but not persistent, just live in the scene
public class SingletonSceneMB<T> : SingletonMB<T> where T : MonoBehaviour
{
    public override bool IsPersistent { get; } = false;
}
