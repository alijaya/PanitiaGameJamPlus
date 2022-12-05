using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Threading;
using Cysharp.Threading.Tasks;

public abstract class AsyncAction : Action
{
    private bool done = false;
    private bool error = false;
    private CancellationTokenSource ct;

    public async override void OnStart()
    {
        ct = new CancellationTokenSource();
        done = false;
        error = false;
        try
        {
            await Progress().AttachExternalCancellation(ct.Token);
        } catch
        {
            error = true;
        }
        done = true;
    }

    // Call Success() or return to indicate Success, will break the async
    // Call Failure() or throw to indicate Failure, will break the async
    public abstract UniTask Progress();

    public void Success()
    {
        error = false;
        ct.Cancel();
    }

    public void Failure()
    {
        error = true;
        ct.Cancel();
    }

    public override TaskStatus OnUpdate()
    {
        if (done)
        {
            if (error)
            {
                return TaskStatus.Failure;
            } else
            {
                return TaskStatus.Success;
            }
        } else
        {
            return TaskStatus.Running;
        }
    }
}
