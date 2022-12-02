using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
[TaskDescription("Walk Customer")]
public class ActionWalkTo : Action
{
    public SharedGameObject targetGameObject;
    public SharedTransform target;

    private ObjectPathFinder pathFinder;
    private GameObject prevGameObject;

    private bool done = false;
    private bool error = false;

    public override void OnStart()
    {
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        if (currentGameObject != prevGameObject)
        {
            pathFinder = currentGameObject.GetComponent<ObjectPathFinder>();
            prevGameObject = currentGameObject;
        }

        if (pathFinder == null) error = true;

        if (error) return;
        pathFinder.OnReached.AddListener(OnReached);
        pathFinder.GoTo(target.Value);
    }

    public void OnReached()
    {
        pathFinder.OnReached.RemoveListener(OnReached);
        done = true;
    }

    public override TaskStatus OnUpdate()
    {
        return error ? TaskStatus.Failure : done ? TaskStatus.Success : TaskStatus.Running;
    }

}
