using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

[RequireComponent(typeof(PathFinder))]
public class Chef : MonoBehaviour {

    private PathFinder pathfinder;

    private AsyncQueue asyncQueue = new();

    private void Awake() {
        pathfinder = GetComponent<PathFinder>();
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

    public async UniTask WaitForSeconds(float seconds, CancellationToken ct = default) {
        await asyncQueue.Queue(()=>WaitTask(seconds), ct);
    }

    private async UniTask WaitTask(float duration) {
        var endTime = Time.time + duration;
        while (Time.time < endTime) {
            await Task.Yield();
        }
    }

}
