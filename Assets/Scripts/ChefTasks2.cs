using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[RequireComponent(typeof(PathFinder2))]
public class ChefTasks2 : MonoBehaviour {

    private PathFinder2 pathfinder;

    private AsyncQueue asyncQueue = new();

    private void Awake() {
        pathfinder = GetComponent<PathFinder2>();
    }

    public async UniTask GoToWorld(Transform target, CancellationToken ct = default)
    {
        await asyncQueue.Queue(() => pathfinder.GoToWorld(target, ct), ct);
    }

    public async UniTask GoToWorld(Vector3 target, CancellationToken ct = default)
    {
        await asyncQueue.Queue(() => pathfinder.GoToWorld(target, ct), ct);
    }

    public async UniTask GoTo(CustomCoordinate target, CancellationToken ct = default)
    {
        await asyncQueue.Queue(() => pathfinder.GoTo(target, ct), ct);
    }

    public async UniTask GoTo(Vector3 target, CancellationToken ct = default)
    {
        await asyncQueue.Queue(() => pathfinder.GoTo(target, ct), ct);
    }

}
