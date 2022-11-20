using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using RS.Typing.Core;

[RequireComponent(typeof(ObjectPathFinder), typeof(MovementController))]
public class Customer : MonoBehaviour
{
    public Transform door { get; private set; }
    public PointOfInterest targetPosition { get; private set; }

    public float seatingDuration = .5f;

    public Canvas orderUI;

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

    public void Setup(Transform door, PointOfInterest targetPosition)
    {
        this.door = door;
        this.targetPosition = targetPosition;

        orderUI.gameObject.SetActive(false);
        movement.SetFaceLeft(false);
        transform.position = door.position;

        targetPosition.occupyObject = gameObject;

        pathfinder.OnReached.AddListener(OnReachSeating);
        pathfinder.GoTo(this.targetPosition.transform);
    }

    public void SetOder(IEnumerable<ItemSO> items) {
        var order = orderUI.GetComponent<Order>();
        order.Setup(items);
    }

    public void Leave()
    {
        if (IsArrived)
        {
            orderUI.gameObject.SetActive(false);

            IsArrived = false;
            this.targetPosition.occupyObject = null;
            UnSeat();
            pathfinder.OnReached.AddListener(OnReachDoor);
            pathfinder.GoTo(this.door);
        }
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
        orderUI.gameObject.SetActive(true);
    }

    private void OnReachDoor()
    {
        pathfinder.OnReached.RemoveListener(OnReachDoor);
        Destroy(gameObject);
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
