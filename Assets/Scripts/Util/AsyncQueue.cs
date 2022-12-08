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

    public async UniTask Queue(Func<UniTask> task)
    {
        // get the waiting number
        var currentIndex = queueTailIndex++;

        // if queueHeadIndex is not currentIndex, it means it still processes another request
        await UniTask.WaitUntil(() => queueHeadIndex == currentIndex); // wait until we process this request

        // actual stuff To Do
        await task();
        queueHeadIndex++; // advance the process number

        if (queueHeadIndex == queueTailIndex)
        {
            // if this is the last task, well reset it, so we don't get big number
            queueHeadIndex = 0;
            queueTailIndex = 0;
        }
    }

}
