using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CallChefToHere : MonoBehaviour
{
    public async UniTask Go(CancellationToken ct = default)
    {
        var poi = GetComponent<PointOfInterest>();
        if (poi)
        {
            await Chef.I.GoToPOI(poi, ct);
        } else
        {
            await Chef.I.GoToWorld(transform, ct);
        }
    }
}
