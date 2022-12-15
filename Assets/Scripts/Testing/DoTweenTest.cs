using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoTweenTest : MonoBehaviour
{
    public float speed = 1;

    private Tween tween;

    private void OnDisable()
    {
        tween?.Kill();
    }

    public void GoTo(Vector3 position)
    {
        tween?.Kill();
        tween = transform.DOMove(position, speed).SetSpeedBased().SetEase(Ease.Linear);
    }
}
