using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovementController : MonoBehaviour
{
    public float speed = 1;
    public SpriteRenderer sprite;

    public float bobHeight = 0.01f;
    public float bobRotate = 10f;
    public float bobFreq = 0.3f;

    public float rotateDuration = .3f;

    private float t = 0;

    private float old_dx = 0;
    private float old_dy = 0;

    private float direction = 1;

    private void Start()
    {
        t = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        var speedDelta = speed * Time.deltaTime;
        var dx = 0f;
        var dy = 0f;
        if (Input.GetKey(KeyCode.DownArrow))
        {
            dy -= 1;
        }
        if (Input.GetKey(KeyCode.UpArrow)) {
            dy += 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            dx -= 1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            dx += 1;
        }
        var d = Mathf.Sqrt(dx * dx + dy * dy);
        transform.Translate(dx * speedDelta, dy * .5f * speedDelta, 0);

        t += d * speedDelta;

        //sprite.transform.localPosition = new Vector3(0, bobHeight * Mathf.Sin(t * bobFreq * 2 * Mathf.PI), 0);
        var rotation = sprite.transform.localEulerAngles;
        rotation.z = bobRotate * Mathf.Sin(t * bobFreq * 2 * Mathf.PI);

        if (dx * direction < 0)
        {
            var yRot = 0f;
            if (dx < 0) yRot = 180f;
            DOTween.To(() => sprite.transform.localEulerAngles.y, (value) =>
            {
                var rot = sprite.transform.localEulerAngles;
                rot.y = value;
                sprite.transform.localEulerAngles = rot;
            }, yRot, rotateDuration);
        }
        sprite.transform.localEulerAngles = rotation;

        old_dx = dx;
        old_dy = dy;

        if (Mathf.Abs(dx) > 0)
        {
            direction = dx;
        }
    }
}
