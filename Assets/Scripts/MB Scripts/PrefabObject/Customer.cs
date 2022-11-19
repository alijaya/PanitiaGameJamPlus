using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(ObjectPathFinder), typeof(MovementController))]
public class Customer : MonoBehaviour
{
    public PointOfInterest targetPosition { get; private set; }

    public float seatingDuration = .5f;

    private ObjectPathFinder pathfinder;
    private MovementController movement;
    private SpriteRenderer sprite;

    public bool IsSeating { get; private set; } = false;
    public bool IsArrived { get; private set; } = false;

    private void Awake()
    {
        pathfinder = GetComponent<ObjectPathFinder>();
        movement = GetComponent<MovementController>();
        sprite = movement.sprite;
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

        IsArrived = true;
        if (targetPosition.isChair)
        {
            Seat();
        }
        targetPosition = null;
    }

    public void Seat() { 
        if (!IsSeating)
        {
            IsSeating = true;
            sprite.transform.DOLocalMoveY(targetPosition.seatHeight, seatingDuration);
        }
    }

    public void UnSeat()
    {
        if (IsSeating)
        {
            IsSeating = false;
            sprite.transform.DOLocalMoveY(0, seatingDuration);
        }
    }
}
