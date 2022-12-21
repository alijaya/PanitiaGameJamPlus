using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class AsyncQueue
{

    private int queueHeadIndex = 0;
    private int queueTailIndex = 0;

    public async UniTask Queue(Func<UniTask> task, CancellationToken ct = default)
    {
        // get the waiting number
        var currentIndex = queueTailIndex++;

        // if queueHeadIndex is not currentIndex, it means it still processes another request
        var cancel = await UniTask.WaitUntil(() => queueHeadIndex == currentIndex, PlayerLoopTiming.Update, ct).SuppressCancellationThrow(); // wait until we process this request

        // actual stuff To Do
        if (!cancel) await task();
        queueHeadIndex++; // advance the process number

        if (queueHeadIndex == queueTailIndex)
        {
            // if this is the last task, well reset it, so we don't get big number
            queueHeadIndex = 0;
            queueTailIndex = 0;
        }
    }

}
