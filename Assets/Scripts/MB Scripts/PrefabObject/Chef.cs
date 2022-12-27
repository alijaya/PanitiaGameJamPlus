using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

[RequireComponent(typeof(PathFinder))]
public class Chef : SingletonSceneMB<Chef> {

    private PathFinder pathfinder;
    private MovementController movement;
    private CustomCoordinate coordinate;

    private AsyncQueue asyncQueue = new();

    protected override void SingletonAwakened()
    {
        pathfinder = GetComponent<PathFinder>();
        movement = GetComponent<MovementController>();
        coordinate = GetComponent<CustomCoordinate>();
    }

    // TODO: damn butuh di refactor, ini kopas dari Customer

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

    // This is in World Coordinate
    public void SetToWorld(Transform target)
    {
        coordinate.SetToWorld(target);
    }

    // This is in World Coordinate
    public void SetToWorld(Vector3 target)
    {
        coordinate.SetToWorld(target);
    }

    // This is in Game Coordinate
    public void SetTo(CustomCoordinate target)
    {
        coordinate.SetTo(target);
    }

    // This is in Game Coordinate
    public void SetTo(Vector3 target)
    {
        coordinate.SetTo(target);
    }

    public async UniTask Seat(float height, CancellationToken ct = default)
    {
        var curPos = coordinate.position;
        curPos.z = height;
        await movement.GoTo(curPos, ct);
    }

    public async UniTask Unseat(CancellationToken ct = default)
    {
        var curPos = coordinate.position;
        curPos.z = 0;
        await movement.GoTo(curPos, ct);
    }

    // This is in World Coordinate
    public async UniTask GoToPOI(PointOfInterest poi, CancellationToken ct = default)
    {
        await asyncQueue.Queue(async () =>
        {
            await Unseat(ct);
            await pathfinder.GoTo(poi.GetComponent<CustomCoordinate>(), ct);
            if (poi.stopDirection == PointOfInterest.StopDirection.Left)
            {
                await SetFaceLeft();
            }
            else if (poi.stopDirection == PointOfInterest.StopDirection.Right)
            {
                await SetFaceRight();
            }
            if (poi.isSeat) await Seat(poi.seatHeight, ct);
        }, ct);
    }

    public async UniTask SetFaceLeft(bool animated = true)
    {
        await movement.SetFaceLeft(animated);
    }

    public async UniTask SetFaceRight(bool animated = true)
    {
        await movement.SetFaceRight(animated);
    }

    public async UniTask SetFacing(float deltaX, bool animated = true)
    {
        await movement.SetFacing(deltaX, animated);
    }

    public async UniTask SetFacing(bool left, bool animated = true)
    {
        await movement.SetFacing(left, animated);
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
