using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    public static UniTask ToUniTask(this UnityEvent unityEvent, CancellationToken ct = default)
    {
        var utcs = new UniTaskCompletionSource();

        UnityAction action = default;
        CancellationTokenRegistration unreg = default;

        action = () =>
        {
            unreg.Dispose();
            unityEvent.RemoveListener(action);
            utcs.TrySetResult();
        };
        unityEvent.AddListener(action);

        unreg = ct.Register(() =>
        {
            unreg.Dispose();
            unityEvent.RemoveListener(action);
            utcs.TrySetCanceled(ct);
        });

        return utcs.Task;
    }

    public static UniTask<T0> ToUniTask<T0>(this UnityEvent<T0> unityEvent, CancellationToken ct = default)
    {
        var utcs = new UniTaskCompletionSource<T0>();

        UnityAction<T0> action = default;
        CancellationTokenRegistration unreg = default;

        action = (value0) =>
        {
            unreg.Dispose();
            unityEvent.RemoveListener(action);
            utcs.TrySetResult(value0);
        };
        unityEvent.AddListener(action);

        unreg = ct.Register(() =>
        {
            unreg.Dispose();
            unityEvent.RemoveListener(action);
            utcs.TrySetCanceled(ct);
        });

        return utcs.Task;
    }

    public static UniTask<(T0, T1)> ToUniTask<T0, T1>(this UnityEvent<T0, T1> unityEvent, CancellationToken ct = default)
    {
        var utcs = new UniTaskCompletionSource<(T0, T1)>();

        UnityAction<T0, T1> action = default;
        CancellationTokenRegistration unreg = default;

        action = (value0, value1) =>
        {
            unreg.Dispose();
            unityEvent.RemoveListener(action);
            utcs.TrySetResult((value0, value1));
        };
        unityEvent.AddListener(action);

        unreg = ct.Register(() =>
        {
            unreg.Dispose();
            unityEvent.RemoveListener(action);
            utcs.TrySetCanceled(ct);
        });

        return utcs.Task;
    }
}
