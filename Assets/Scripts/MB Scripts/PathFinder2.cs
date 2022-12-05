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

    public async UniTask GoTo(Transform target, CancellationToken ct = default)
    {
        await GoTo(target.position, ct);
    }

    public async UniTask GoTo(Vector3 target, CancellationToken ct = default)
    {
        Stop();

        path = Pathfinding.I.FindPath(this.transform.position, target);
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
