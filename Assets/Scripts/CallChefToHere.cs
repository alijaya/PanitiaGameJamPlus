using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class CallChefToHere : MonoBehaviour
{
    public Transform location;

    private ChefTasks2 chef;

    private void Awake()
    {
        chef = FindObjectOfType<ChefTasks2>();
    }

    public async UniTask Go()
    {
        await chef.GoTo(location);
    }
}
