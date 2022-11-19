using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class ObjectPathFinder : MonoBehaviour
{
    private Transform target;
    private Vector3 targetPosition;
    private Vector2[] path;
    private int currentPathIndex;

    public bool IsMoving { get; private set; }

    public UnityEvent OnReached;
    public UnityEvent OnInterrupted;
    public UnityEvent OnStop;
    public UnityEvent OnStart;

    private MovementController movement;

    private void Awake()
    {
        movement = GetComponent<MovementController>();
    }

    private void OnEnable()
    {
        movement.OnReached.AddListener(OnMovementReached);
        movement.OnInterrupted.AddListener(OnMovementInterrupted);
    }

    private void OnDisable()
    {
        movement.OnReached.RemoveListener(OnMovementReached);
        movement.OnInterrupted.RemoveListener(OnMovementInterrupted);
    }

    // Start is called before the first frame update
    void Start()
    {
        IsMoving = false;
    }

    public void GoTo(Vector3 position)
    {
        if (IsMoving)
        {
            OnInterrupted.Invoke();
            OnStop.Invoke();
        }
        target = null;
        targetPosition = position;
        IsMoving = true;
        path = Pathfinding.I.FindPath(transform.position, targetPosition);
        currentPathIndex = 0;
        OnStart.Invoke();
        GoToCurrentWaypoint();
    }

    public void GoTo(Transform transform)
    {
        if (IsMoving)
        {
            OnInterrupted.Invoke();
            OnStop.Invoke();
        }
        target = transform;
        targetPosition = target.position;
        IsMoving = true;
        path = Pathfinding.I.FindPath(transform.position, targetPosition);
        currentPathIndex = 0;
        OnStart.Invoke();
        GoToCurrentWaypoint();
    }

    public void Stop()
    {
        if (IsMoving)
        {
            target = null;
            targetPosition.Set(0, 0, 0);
            IsMoving = false;
            movement.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMoving)
        {
            if (target)
            {
                if (targetPosition != target.position)
                {
                    GoTo(target); // refresh again
                }
            }
        }
    }

    private void OnMovementReached()
    {
        if (IsMoving)
        {
            currentPathIndex++;
            if (currentPathIndex >= path.Length) // finish
            {
                IsMoving = false;
                OnReached.Invoke();
                OnStop.Invoke();
            }
            else
            {
                GoToCurrentWaypoint();
            }
        }
    }

    private void OnMovementInterrupted()
    {
        if (IsMoving)
        {
            IsMoving = false;
            OnInterrupted.Invoke();
            OnStop.Invoke();
        }
    }

    private void GoToCurrentWaypoint()
    {
        if (path.Length > 0 && currentPathIndex < path.Length)
        {
            Vector2 currentWaypoint = path[currentPathIndex];

            movement.GoTo(currentWaypoint);
        }
    }
}
