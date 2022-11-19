using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    private float t = 0;

    private float direction = 1; // 1 = right, -1 = left

    private void Start()
    {
        t = 0;
        IsMoving = false;
    }

    public void GoTo(Vector3 position)
    {
        target = null;
        targetPosition = position;
        IsMoving = true;
    }

    public void GoTo(Transform transform)
    {
        target = transform;
        targetPosition = target.position;
        IsMoving = true;
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
        if (newDirection * direction < 0)
        {
            var yRot = 0f;
            if (newDirection < 0) yRot = 180f;
            DOTween.To(() => sprite.transform.localEulerAngles.y, (value) =>
            {
                var rot = sprite.transform.localEulerAngles;
                rot.y = value;
                sprite.transform.localEulerAngles = rot;
            }, yRot, rotateDuration);
        }
        sprite.transform.localEulerAngles = rotation;

        if (Mathf.Abs(newDirection) > 0)
        {
            direction = newDirection;
        }
    }
}
