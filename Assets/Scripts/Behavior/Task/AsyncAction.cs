using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Threading;
using Cysharp.Threading.Tasks;

public abstract class AsyncAction : Action
{
    //private bool ongoing = false;
    private bool done = false;
    private bool error = false;

    private CancellationTokenSource pauseToken;

    public override void OnPause(bool paused)
    {
        if (paused)
        {
            pauseToken?.Cancel();
            pauseToken?.Dispose();
            pauseToken = null;
        } else
        {
            
            StartAsync().Forget();
        }
    }

    public async UniTask StartAsync()
    {
        // if it's ongoing, then cancel the previous
        pauseToken?.Cancel();

        pauseToken?.Dispose();
        pauseToken = new CancellationTokenSource();

        //ongoing = true;
        done = false;
        error = false;
        var (cancel, result) = await Progress(pauseToken.Token).SuppressCancellationThrow();
        error = cancel || !result;
        done = true;
        //ongoing = false;
    }

    public override void OnStart()
    {
        StartAsync().Forget();
    }

    public override void OnEnd()
    {
        pauseToken?.Cancel();
        pauseToken?.Dispose();
        pauseToken = null;
    }

    // return true: success, return false: failure
    public abstract UniTask<bool> Progress(CancellationToken ct);

    public override TaskStatus OnUpdate()
    {
        //if (!ongoing && !done) StartAsync().Forget();

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
