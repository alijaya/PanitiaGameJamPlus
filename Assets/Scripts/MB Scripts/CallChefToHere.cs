using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CallChefToHere : MonoBehaviour
{
    private Chef chef;

    private void Awake()
    {
        chef = FindObjectOfType<Chef>();
    }

    public async UniTask Go(CancellationToken ct = default)
    {
        await chef.GoToWorld(transform, ct);
    }
}
