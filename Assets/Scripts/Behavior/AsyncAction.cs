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

    private CancellationTokenSource pauseToken = new CancellationTokenSource();

    public override void OnPause(bool paused)
    {
        if (paused)
        {
            pauseToken.Cancel();
        } else
        {
            if(pauseToken == null)
            {
                pauseToken.Dispose();
            }
            pauseToken = new CancellationTokenSource();
        }
    }

    public async override void OnStart()
    {
        done = false;
        error = false;
        var (cancel, result) = await Progress(pauseToken.Token).SuppressCancellationThrow();
        error = cancel || !result;
        done = true;
    }

    // return true: success, return false: failure
    public abstract UniTask<bool> Progress(CancellationToken ct);

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
