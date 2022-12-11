using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cysharp.Threading.Tasks;
using System.Threading;

[TaskName("Walk To")]
[TaskCategory("Game")]
[TaskDescription("Walk Customer")]
public class ActionWalkTo : AsyncAction
{
    public SharedGameObject targetGameObject;
    public SharedTransform target;

    private PathFinder pathFinder;
    private MovementController movement;
    private GameObject prevGameObject;

    public async override UniTask<bool> Progress(CancellationToken ct)
    {
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        if (currentGameObject != prevGameObject)
        {
            movement = currentGameObject.GetComponent<MovementController>();
            prevGameObject = currentGameObject;
        }

        if (movement == null) return false;

        await movement.GoToWorld(target.Value, ct);
        return true;
    }
}
