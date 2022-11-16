using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using Eflatun.SceneReference;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GlobalRef : SingletonSO<GlobalRef>
{
    public int globalInt = 5; // example of global stuff
}
