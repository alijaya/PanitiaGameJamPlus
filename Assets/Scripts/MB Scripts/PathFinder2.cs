using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(MovementController2), typeof(CustomCoordinate))]
public class PathFinder2 : MonoBehaviour
{
    private Vector2[] path;
    private int currentPathIndex;

    private CancellationTokenSource lastCT;

    public bool IsMoving { get; private set; }

    private MovementController2 movement;
    private CustomCoordinate coordinate;

    private void Awake()
    {
        movement = GetComponent<MovementController2>();
        coordinate = GetComponent<CustomCoordinate>();
    }

    public async UniTask GoToWorld(Transform target, CancellationToken ct = default)
    {
        await GoToWorld(target.position, ct);
    }

    public async UniTask GoToWorld(Vector3 target, CancellationToken ct = default)
    {
        await GoTo(CustomCoordinate.WorldToGameCoordinate(target), ct);
    }

    // This is in Game World Coordinate
    public async UniTask GoTo(CustomCoordinate target, CancellationToken ct = default)
    {
        await GoTo(target.position, ct);
    }

    // This is in Game World Coordinate
    public async UniTask GoTo(Vector3 target, CancellationToken ct = default)
    {
        Stop();

        path = Pathfinding.I.FindPath(coordinate.position, target);
        currentPathIndex = 0;

        IsMoving = true;
        while (IsMoving && path.Length > 0 && currentPathIndex < path.Length)
        {
            Vector2 currentWaypoint = path[currentPathIndex++];

            await movement.GoTo(currentWaypoint, ct);
        }
        IsMoving = false;
    }

    public void Stop()
    {
        if (IsMoving)
        {
            IsMoving = false;
            movement.Stop();
        }
    }
}
