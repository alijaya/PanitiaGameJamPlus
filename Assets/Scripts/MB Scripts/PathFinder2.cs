using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(MovementController2))]
public class PathFinder2 : MonoBehaviour
{
    private Vector2[] path;
    private int currentPathIndex;

    private CancellationTokenSource lastCT;

    public bool IsMoving { get; private set; }

    private MovementController2 movement;

    private void Awake()
    {
        movement = GetComponent<MovementController2>();
    }

    // This is in World Coordinate
    public async UniTask GoToWorld(Transform target, CancellationToken ct = default)
    {
        await GoToWorld(target.position, ct);
    }

    // This is in World Coordinate
    public async UniTask GoToWorld(Vector3 target, CancellationToken ct = default)
    {
        Stop();

        path = Pathfinding.I.FindPath(transform.position, target);
        currentPathIndex = 0;

        IsMoving = true;
        while (IsMoving && path.Length > 0 && currentPathIndex < path.Length)
        {
            Vector2 currentWaypoint = path[currentPathIndex++];

            await movement.GoToWorld(currentWaypoint, ct);
        }
        IsMoving = false;
    }

    // This is in Game Coordinate
    public async UniTask GoTo(CustomCoordinate target, CancellationToken ct = default)
    {
        await GoTo(target.position, ct);
    }

    // This is in Game Coordinate
    public async UniTask GoTo(Vector3 target, CancellationToken ct = default)
    {
        await GoToWorld(CustomCoordinate.GameToWorldCoordinate(target), ct);
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
