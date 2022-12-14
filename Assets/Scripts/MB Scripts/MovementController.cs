using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Sirenix.OdinInspector;

[RequireComponent(typeof(CustomCoordinate))]
public class MovementController : MonoBehaviour
{
    public Transform sprite;

    [SuffixLabel("unit / s", true)]
    public float speed = 5;
    [SuffixLabel("turn / s", true)]
    public float rotateSpeed = 2;
    [SuffixLabel("degree", true)]
    public float bobRotate = 10;
    [SuffixLabel("degree / s", true)]
    public float bobSpeed = 40;

    private Tween movementTween;
    private Tween flipTween;
    private Tween bobTween;
    private bool bobDirection = true;

    public CustomCoordinate coordinate { get; private set; }

    public bool IsMoving
    {
        get
        {
            return movementTween != null && movementTween.IsActive() && movementTween.IsPlaying();
        }
    }

    private void Awake()
    {
        coordinate = GetComponent<CustomCoordinate>();
    }

    private void OnDisable()
    {
        // stop mid way
        movementTween?.Kill();

        // complete the flip
        flipTween?.Complete();

        // complete the bob
        bobTween?.Complete();
    }

    // This is in World Coordinate
    public async UniTask GoToWorld(Transform target, CancellationToken ct = default)
    {
        await GoToWorld(target.position, ct);
    }

    // This is in World Coordinate
    public async UniTask GoToWorld(Vector3 target, CancellationToken ct = default)
    {
        await GoTo(CustomCoordinate.WorldToGameCoordinate(target), ct);
    }

    // This is in Game Coordinate
    public async UniTask GoTo(CustomCoordinate target, CancellationToken ct = default)
    {
        await GoTo(target.position, ct);
    }

    // This is in Game Coordinate
    public async UniTask GoTo(Vector3 target, CancellationToken ct = default)
    {
        Stop();

        movementTween = DOTween.To(() => coordinate.position, v => coordinate.position = v, target, speed).SetSpeedBased().SetEase(Ease.Linear);

        SetFacing(target.x - transform.position.x).Forget();
        StartBobbing().Forget();

        // wait when it's completed or killed
        var cancelled = await movementTween.WithCancellation(ct).SuppressCancellationThrow();

        movementTween = null;
        // if stopped not complete, means stop prematurely
        // check if stopped by the caller from CancellationToken
        // if not it means stopped internally by error, Calling Stop, or object destroyed / disabled
        // somehow IsActive is true if it's stopped prematurely, and false when completed
        if (cancelled && !ct.IsCancellationRequested)
        {
            throw new OperationCanceledException(); // propagate
        }
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
        // force stop
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
