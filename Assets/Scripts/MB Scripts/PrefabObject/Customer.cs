using System.Collections.Generic;
using Core;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(PathFinder2), typeof(MovementController2), typeof(CustomCoordinate))]
public class Customer : MonoBehaviour
{
    public Transform door { get; private set; }
    public PointOfInterest targetPosition { get; private set; }

    public float seatingDuration = .5f;

    public GameObject orderUI;

    private PathFinder2 pathfinder;
    private MovementController2 movement;
    private CustomCoordinate coordinate;
    public SpriteRenderer sprite;

    public bool IsSeating { get; private set; } = false;
    public bool IsArrived { get; private set; } = false;

    private void Awake()
    {
        pathfinder = GetComponent<PathFinder2>();
        movement = GetComponent<MovementController2>();
        coordinate = GetComponent<CustomCoordinate>();
    }

    public void Setup(Transform door, PointOfInterest targetPosition)
    {
        this.door = door;
        this.targetPosition = targetPosition;

        orderUI.gameObject.SetActive(false);
        movement.SetFaceLeft(false).Forget();
        coordinate.SetToWorld(door);

        targetPosition.occupyObject = gameObject;

        GlobalRef.I.PlaySFX_CustomerEnter();
        GoToSeating(this.targetPosition.transform).Forget();
    }

    public void SetOrder(IEnumerable<ItemSO> items) {
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
            GoToDoor().Forget();
        }
    }

    private async UniTask GoToSeating(Transform position)
    {
        await pathfinder.GoToWorld(position);

        switch(targetPosition.stopDirection)
        {
            case PointOfInterest.StopDirection.Left:
                await movement.SetFaceLeft();
                break;
            case PointOfInterest.StopDirection.Right:
                await movement.SetFaceRight();
                break;
            default:
                break;

        }

        if (targetPosition.isChair)
        {
            await Seat();
        }

        IsArrived = true;
        GlobalRef.I.PlaySFX_CustomerOrder();
        orderUI.gameObject.SetActive(true);
    }

    private async UniTask GoToDoor()
    {
        await UnSeat();
        await pathfinder.GoToWorld(this.door);
        Destroy(gameObject);
    }

    public async UniTask Seat() {
        var pos = movement.coordinate.position;
        pos.z = targetPosition.seatHeight;
        await movement.GoTo(pos);
        IsSeating = true;
    }

    public async UniTask UnSeat()
    {
        var pos = movement.coordinate.position;
        pos.z = 0;
        await movement.GoTo(pos);
        IsSeating = false;
    }
}
