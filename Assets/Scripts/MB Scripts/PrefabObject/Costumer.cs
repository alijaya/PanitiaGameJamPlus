using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPathFinder), typeof(MovementController))]
public class Costumer : MonoBehaviour
{
    public PointOfInterest targetPosition { get; private set; }

    private ObjectPathFinder pathfinder;
    private MovementController movement;

    private void Awake()
    {
        pathfinder = GetComponent<ObjectPathFinder>();
        movement = GetComponent<MovementController>();
    }

    public void Setup(PointOfInterest targetPosition)
    {
        this.targetPosition = targetPosition;
        targetPosition.occupyObject = gameObject;
        movement.SetFaceLeft(false);
        pathfinder.OnReached.AddListener(OnReachSeating);
        pathfinder.GoTo(this.targetPosition.transform);
    }

    private void OnReachSeating()
    {
        pathfinder.OnReached.RemoveListener(OnReachSeating);
        switch(targetPosition.stopDirection)
        {
            case PointOfInterest.StopDirection.Left:
                movement.SetFaceLeft();
                break;
            case PointOfInterest.StopDirection.Right:
                movement.SetFaceRight();
                break;
            default:
                break;

        }
        targetPosition = null;
    }
}
