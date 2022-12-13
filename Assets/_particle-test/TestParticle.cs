using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TestParticle : MonoBehaviour
{
    private void Awake()
    {
        // transform.DOMoveX(5f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        transform.DOJump(new Vector3(5f, 0f), 2, -1, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
