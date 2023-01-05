using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class AsyncQueue
{
    private UniTask lastTask = UniTask.CompletedTask;

    // can not cancel ._.
    public async UniTask Queue(Func<UniTask> task)
    {
        // push task
        var curTask = lastTask;
        var lastTaskSource = new UniTaskCompletionSource();
        lastTask = lastTaskSource.Task;

        // should not have error here, always wait until completion
        await curTask;

        try
        {
            await task();
        } finally
        {
            lastTaskSource.TrySetResult();
        }
    }

}
