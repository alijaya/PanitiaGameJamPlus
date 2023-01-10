using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Threading;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class PathFinder : MonoBehaviour
{
    private Vector2[] path;
    private int currentPathIndex;

    public bool IsMoving { get; private set; }

    private MovementController movement;

    private void Awake()
    {
        movement = GetComponent<MovementController>();
    }

    private void OnDisable()
    {
        Stop();
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
        bool cancel = false;
        while (path.Length > 0 && currentPathIndex < path.Length)
        {
            Vector2 currentWaypoint = path[currentPathIndex++];

            // if last waypoint, then go directly to target
            if (currentPathIndex == path.Length)
            {
                currentWaypoint = target;
            }

            cancel = await movement.GoToWorld(currentWaypoint, ct).SuppressCancellationThrow();

            // if stopped not complete, means stop prematurely
            // check if stopped by the caller from CancellationToken
            // if not it means stopped internally by error, Calling Stop, or object destroyed / disabled
            if ((cancel || !IsMoving) && !ct.IsCancellationRequested)
            {
                Stop();
                throw new OperationCanceledException();
            }

            // if requested to stop, then stop
            if (ct.IsCancellationRequested) break;
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
