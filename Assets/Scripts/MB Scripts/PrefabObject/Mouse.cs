using RS.Typing.Core;
using UnityEngine;

public class Mouse : MonoBehaviour {
    private ObjectPathFinder pathfinder;
    private MovementController movement;
    private Item _targetedItem;

    public Transform doorTransform;
    private void Awake() {
        pathfinder = GetComponent<ObjectPathFinder>();
        movement = GetComponent<MovementController>();
        _targetedItem = ItemTray.Instance.GetRandomItem();
    }

    private void Start() {
        if (_targetedItem && _targetedItem.StackSize > 0) {
            pathfinder.OnReached.AddListener(OnReachObjective);
            MoveTo(_targetedItem.transform);
        }
    }

    private void OnReachObjective() {
        pathfinder.OnReached.RemoveListener(OnReachObjective);
        _targetedItem.ReduceStack();
        pathfinder.OnReached.AddListener(OnReachDoor);
        pathfinder.GoTo(doorTransform);
    }

    private void OnReachDoor()
    {
        pathfinder.OnReached.RemoveListener(OnReachDoor);
        Destroy(gameObject);
    }


    private void MoveTo(Transform objectiveTransform) {
        pathfinder.GoTo(objectiveTransform);
    }
}