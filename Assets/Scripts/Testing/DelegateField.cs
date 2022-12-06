using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

public class DelegateField : SerializedMonoBehaviour
{
    public Func<UniTask> fun;

    private async UniTask Start()
    {
        if (fun != null)
        {
            await fun();
        }
        Debug.Log("Hey!");
    }

    public async UniTask Wait()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2));
    }
}
