using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChefTasks : MonoBehaviour {

    private ObjectPathFinder pathfinder;
    private MovementController movement;

    private readonly Queue<KeyValuePair<Transform,Action>> _actionsQueue = new ();

    private bool _isReached;

    private void Awake() {
        pathfinder = GetComponent<ObjectPathFinder>();
        movement = GetComponent<MovementController>();
    }

    private void OnEnable() {
        pathfinder.OnReached.AddListener(OnReachObjective);
    }

    private void OnDisable() {
        pathfinder.OnReached.RemoveListener(OnReachObjective);
    }

    private void OnReachObjective() {
        _isReached = true;
    }

    private void Start() {
        StartCoroutine(DoTask());
    }

    private IEnumerator DoTask() {
        while (true) {
            yield return new WaitUntil(() => _actionsQueue.Count > 0);
            
            var task = _actionsQueue.Dequeue();
            if (task.Key != null) {
                var taskLocation = task.Key;
                MoveTo(taskLocation);
                yield return new WaitUntil(() => _isReached);    
            }
            task.Value();
        }
    }

    public void AddTask(KeyValuePair<Transform,Action> task) {
        _actionsQueue.Enqueue(task);
    }

    private void MoveTo(Transform objectiveTransform) {
        _isReached = false;
        var dist = Vector2.Distance(objectiveTransform.position, transform.position);
        if (dist < .1f) {
            _isReached = true;
        }
        else {
            pathfinder.GoTo(objectiveTransform);    
        }
        
    }
}
