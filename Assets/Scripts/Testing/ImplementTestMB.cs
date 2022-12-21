using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImplementTestMB : MonoBehaviour, ITest
{
    public bool TestBool()
    {
        return true;
    }
}
