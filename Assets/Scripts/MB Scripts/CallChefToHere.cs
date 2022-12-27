using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CallChefToHere : MonoBehaviour
{
    public async UniTask Go(CancellationToken ct = default)
    {
        await Chef.I.GoToWorld(transform, ct);
    }
}
