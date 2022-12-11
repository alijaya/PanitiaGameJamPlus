using System;
using Core;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(PathFinder2), typeof(MovementController2), typeof(CustomCoordinate))]
public class Mouse : MonoBehaviour {
    private PathFinder2 pathfinder;
    private MovementController2 movement;
    private CustomCoordinate coordinate;
    private Item _targetedItem;

    public Transform doorTransform;
    private bool _hasItem;
    private void Awake() {
        pathfinder = GetComponent<PathFinder2>();
        movement = GetComponent<MovementController2>();
        coordinate = GetComponent<CustomCoordinate>();
    }

    private void Start() {
        coordinate.SetToWorld(doorTransform);
        GoToObjective(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask GoToObjective(CancellationToken ct) {

        while (_targetedItem == null || _targetedItem.StackSize == 0) {
            _targetedItem = ItemTray.Instance.GetRandomItem();
            await pathfinder.GoToWorld(_targetedItem.transform, ct);
            if (ct.IsCancellationRequested) return;
        }
        
        _targetedItem.ReduceStack();
        _hasItem = true;
        GoToDoor(ct).Forget();
    }

    private async UniTask GoToDoor(CancellationToken ct)
    {
        await pathfinder.GoToWorld(doorTransform, ct);
        if (ct.IsCancellationRequested) return;
        Destroy(gameObject);
    }


    private async UniTask MoveTo(Transform objectiveTransform, CancellationToken ct)
    {
        await pathfinder.GoToWorld(objectiveTransform, ct);
    }

    public void DestroyedByWord() {
        if (_hasItem) _targetedItem.AddStack();
        Destroy(gameObject);
    }

}