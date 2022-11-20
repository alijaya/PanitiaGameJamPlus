using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChefTasks : MonoBehaviour {
    private readonly List<UnityEvent> tasks = new ();
    
    private ObjectPathFinder pathfinder;
    private MovementController movement;

    private void Awake() {
        pathfinder = GetComponent<ObjectPathFinder>();
        movement = GetComponent<MovementController>();
    }
    private void OnReachObjective() {
        pathfinder.OnReached.RemoveListener(OnReachObjective);
        DoTask();
    }

    private void DoTask() {
        if (tasks.Count < 1) return;
        tasks[0]?.Invoke();
        tasks.RemoveAt(0);
    }

    public void AddTask(UnityEvent task) {
        tasks.Add(task);
    }
    

    public void MoveTo(Transform objectiveTransform) {
        var dist = Vector2.Distance(objectiveTransform.position, transform.position);
        if (dist < .1f) {
            DoTask();
        }
        else {
            pathfinder.OnReached.AddListener(OnReachObjective);
            pathfinder.GoTo(objectiveTransform);    
        }
        
    }
}
