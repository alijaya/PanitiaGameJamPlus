using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System;

public class MovementController : MonoBehaviour
{
    public float speed = 1;
    private Transform target;
    private Vector3 targetPosition;
    public bool IsMoving { get; private set; }

    public SpriteRenderer sprite;

    //public float bobHeight = 0.01f;
    public float bobRotate = 10f;
    public float bobFreq = 0.3f;

    public float rotateDuration = .3f;

    public UnityEvent OnReached;
    public UnityEvent OnInterrupted;
    public UnityEvent OnStop;
    public UnityEvent OnStart;

    private float t = 0;

    private bool isFaceLeft = false;

    private void Awake()
    {
        t = 0;
        IsMoving = false;
    }

    public void GoTo(Vector3 position)
    {
        if (IsMoving)
        {
            OnInterrupted.Invoke();
            OnStop.Invoke();
        }
        target = null;
        targetPosition = position;
        IsMoving = true;
        OnStart.Invoke();
    }

    public void GoTo(Transform transform)
    {
        if (IsMoving)
        {
            OnInterrupted.Invoke();
            OnStop.Invoke();
        }
        target = transform;
        targetPosition = target.position;
        IsMoving = true;
        OnStart.Invoke();
    }

    public void SetFaceLeft(bool animated = true)
    {
        SetFacing(true, animated);
    }

    public void SetFaceRight(bool animated = true)
    {
        SetFacing(false, animated);
    }

    public void SetFacing(bool left, bool animated = true)
    {
        var yRot = 0f;
        if (left) yRot = 180f;

        if (animated)
        {
            if (isFaceLeft != left)
            {
                DOTween.To(() => sprite.transform.localEulerAngles.y, (value) =>
                {
                    var rot = sprite.transform.localEulerAngles;
                    rot.y = value;
                    sprite.transform.localEulerAngles = rot;
                }, yRot, rotateDuration);
            }
        }
        else
        {
            var angle = sprite.transform.localEulerAngles;
            angle.y = yRot;
            sprite.transform.localEulerAngles = angle;
        }

        isFaceLeft = left;
    }

    public void Stop()
    {
        if (IsMoving)
        {
            target = null;
            targetPosition.Set(0, 0, 0);
            IsMoving = false;
            OnInterrupted.Invoke();
            OnStop.Invoke();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsMoving)
        {
            if (target) targetPosition = target.position;
            UpdatePosition();
        }

        if (Vector3.Distance(targetPosition, transform.position) <= 0.001) // epsilon
        {
            target = null;
            targetPosition.Set(0, 0, 0);
            IsMoving = false;
            OnReached.Invoke();
            OnStop.Invoke();
        }
    }

    private void UpdatePosition()
    {
        var speedDelta = speed * Time.deltaTime;

        var delta = targetPosition - transform.position;
        var normalizedDelta = delta.normalized;
        var coordinateNormalizedDelta = Vector3.Scale(normalizedDelta, new Vector3(1, .5f, 0));
        var scaling = coordinateNormalizedDelta.magnitude;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speedDelta * scaling);

        t += speedDelta;

        //sprite.transform.localPosition = new Vector3(0, bobHeight * Mathf.Sin(t * bobFreq * 2 * Mathf.PI), 0);
        var rotation = sprite.transform.localEulerAngles;
        rotation.z = bobRotate * Mathf.Sin(t * bobFreq * 2 * Mathf.PI);

        var newDirection = Mathf.Sign(normalizedDelta.x);

        sprite.transform.localEulerAngles = rotation;

        SetFacing(newDirection < 0);
    }
}
