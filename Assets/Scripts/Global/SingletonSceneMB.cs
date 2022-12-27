using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonSceneMB<T> : SingletonMB<T> where T : MonoBehaviour
{
    public override bool IsPersistent { get; } = false;
}
