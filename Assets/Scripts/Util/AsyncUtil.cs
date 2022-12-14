using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public static class AsyncUtil
{
    public static async UniTask DelayTask(float delaySeconds, Func<UniTask> func, CancellationToken ct = default)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken: ct);
        await func();
    }
}
