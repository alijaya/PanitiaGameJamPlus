using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCycleDebug : MonoBehaviour
{
    private void Awake()
    {
        Print("Awake");
    }

    private void OnEnable()
    {
        Print("OnEnable");
    }

    void Start()
    {
        Print("Start");
    }

    private void OnDisable()
    {
        Print("OnDisable");
    }

    private void OnDestroy()
    {
        Print("OnDestroy");
    }

    private void Print(string value)
    {
        Debug.Log(name + ": " + value + QuitUtil.GetActiveScene());
    }
}
