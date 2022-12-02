using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Sirenix.OdinInspector;

public class MovementController2 : MonoBehaviour
{
    public Transform sprite;

    [SuffixLabel("unit / s", true)]
    public float speed = 1;
    [SuffixLabel("turn / s", true)]
    public float rotateSpeed = 1;
    [SuffixLabel("degree", true)]
    public float bobRotate = 10;
    [SuffixLabel("degree / s", true)]
    public float bobSpeed = 10;

    private Tween movementTween;
    private Tween flipTween;
    private Tween bobTween;
    private bool bobDirection = true;

    public bool IsMoving
    {
        get
        {
            return movementTween != null && !movementTween.IsComplete();
        }
    }

    public async UniTask GoTo(Transform target)
    {
        await GoTo(target.position);
    }

    public async UniTask GoTo(Vector3 target)
    {
        if (movementTween != null)
        {
            movementTween.Kill();
        }

        movementTween = transform.DOMove(target, speed).SetSpeedBased().SetEase(Ease.Linear)
            .OnComplete(() =>
        {
            movementTween = null;
        });
        SetFacing(target.x - transform.position.x).Forget();
        StartBobbing().Forget();

        // onKill
        await movementTween;
    }

    public async UniTask SetFaceLeft(bool animated = true)
    {
        await SetFacing(true, animated);
    }

    public async UniTask SetFaceRight(bool animated = true)
    {
        await SetFacing(false, animated);
    }

    public async UniTask SetFacing(float deltaX, bool animated = true)
    {
        if (deltaX != 0)
        {
            await SetFacing(deltaX < 0, animated);
        }
    }

    public async UniTask SetFacing(bool left, bool animated = true)
    {
        var yRot = 0f;
        if (left) yRot = 180f;

        if (animated)
        {
            if (flipTween != null)
            {
                flipTween.Kill();
            }

            flipTween = DOTween.To(() => sprite.transform.localEulerAngles.y, (value) =>
            {
                var rot = sprite.transform.localEulerAngles;
                rot.y = value;
                sprite.transform.localEulerAngles = rot;
            }, yRot, rotateSpeed * 360).SetSpeedBased().SetEase(Ease.Linear)
                .OnComplete(() =>
            {
                flipTween = null;
            });

            // onKill
            await flipTween;
        }
        else
        {
            var angle = sprite.transform.localEulerAngles;
            angle.y = yRot;
            sprite.transform.localEulerAngles = angle;
        }
    }

    public void Stop()
    {
        if (movementTween != null)
        {
            movementTween.Kill();
            movementTween = null;
        }
    }

    private async UniTask StartBobbing()
    {
        // if it's still on going, then do nothing, continue the last one
        if (bobTween != null) return;

        bobDirection = true;
        while (IsMoving)
        {
            bobTween = DOTween.To(() => sprite.transform.localEulerAngles.z, (value) =>
            {
                var rot = sprite.transform.localEulerAngles;
                rot.z = value;
                sprite.transform.localEulerAngles = rot;
            }, bobDirection ? bobRotate : -bobRotate, bobSpeed).SetSpeedBased().SetEase(easeHalfWaveFun);

            // onKill
            await bobTween;
            bobDirection = !bobDirection;
        }

        bobTween = null;
    }

    static private float easeHalfWaveFun(float time, float duration, float overshootOrAmplitude, float period)
    {
        return Mathf.Sin(time / duration * Mathf.PI);
    }
}
