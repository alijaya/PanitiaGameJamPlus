using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

[RequireComponent(typeof(PathFinder), typeof(MovementController), typeof(CustomCoordinate))]
public class Customer2 : MonoBehaviour
{
    private PathFinder pathfinder;
    private MovementController movement;
    private CustomCoordinate coordinate;

    [NonSerialized]
    public CustomerGroup customerGroup;

    private void Awake()
    {
        pathfinder = GetComponent<PathFinder>();
        movement = GetComponent<MovementController>();
        coordinate = GetComponent<CustomCoordinate>();
    }

    #region movement

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
        await Unseat(ct);
        await pathfinder.GoTo(poi.GetComponent<CustomCoordinate>(), ct);
        if (poi.stopDirection == PointOfInterest.StopDirection.Left)
        {
            await SetFaceLeft();
        } else if (poi.stopDirection == PointOfInterest.StopDirection.Right)
        {
            await SetFaceRight();
        }
        if (poi.isSeat) await Seat(poi.seatHeight, ct);
    }

    // This is in World Coordinate
    public async UniTask GoToWorld(Transform target, CancellationToken ct = default)
    {
        await Unseat(ct);
        await pathfinder.GoToWorld(target, ct);
    }

    // This is in World Coordinate
    public async UniTask GoToWorld(Vector3 target, CancellationToken ct = default)
    {
        await Unseat(ct);
        await pathfinder.GoToWorld(target, ct);
    }

    // This is in Game Coordinate
    public async UniTask GoTo(CustomCoordinate target, CancellationToken ct = default)
    {
        await Unseat(ct);
        await pathfinder.GoTo(target, ct);
    }

    // This is in Game Coordinate
    public async UniTask GoTo(Vector3 target, CancellationToken ct = default)
    {
        await Unseat(ct);
        await pathfinder.GoTo(target, ct);
    }

    // This is in World Coordinate
    public async UniTask GoToWorldDirect(Transform target, CancellationToken ct = default)
    {
        await movement.GoToWorld(target, ct);
    }

    // This is in World Coordinate
    public async UniTask GoToWorldDirect(Vector3 target, CancellationToken ct = default)
    {
        await movement.GoToWorld(target, ct);
    }

    // This is in Game Coordinate
    public async UniTask GoToDirect(CustomCoordinate target, CancellationToken ct = default)
    {
        await movement.GoTo(target, ct);
    }

    // This is in Game Coordinate
    public async UniTask GoToDirect(Vector3 target, CancellationToken ct = default)
    {
        await movement.GoTo(target, ct);
    }

    public void StopMoving()
    {
        movement.Stop();
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

    #endregion
}
