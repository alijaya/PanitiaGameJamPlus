using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cysharp.Threading.Tasks;

[TaskName("Walk To")]
[TaskCategory("Game")]
[TaskDescription("Walk Customer")]
public class ActionWalkTo : AsyncAction
{
    public SharedGameObject targetGameObject;
    public SharedTransform target;

    private PathFinder pathFinder;
    private GameObject prevGameObject;

    public async override UniTask Progress()
    {
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        if (currentGameObject != prevGameObject)
        {
            pathFinder = currentGameObject.GetComponent<PathFinder>();
            prevGameObject = currentGameObject;
        }

        if (pathFinder == null) Failure();

        await pathFinder.GoToWorld(target.Value);
    }
}
