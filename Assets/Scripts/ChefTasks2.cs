using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[RequireComponent(typeof(PathFinder2))]
public class ChefTasks2 : MonoBehaviour {

    private PathFinder2 pathfinder;

    private int queueIndex = 0;

    private void Awake() {
        pathfinder = GetComponent<PathFinder2>();
    }

    public async UniTask GoTo(Transform position, CancellationToken ct = default)
    {
        await UniTask.WaitUntil(() => queueIndex == 0);

        queueIndex++;
        await pathfinder.GoToWorld(position, ct);
        queueIndex--;
    }

}
