using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class TestAsyncMovement : MonoBehaviour
{
    public Transform target;
    public Transform target2;
    public float speed = 1;

    async UniTaskVoid Start()
    {
        await transform.DOMove(target.position, speed).SetSpeedBased().SetEase(Ease.Linear);
        Debug.Log("half");
        await transform.DOMove(target2.position, speed).SetSpeedBased().SetEase(Ease.Linear);
        Debug.Log("finish");
    }
}
