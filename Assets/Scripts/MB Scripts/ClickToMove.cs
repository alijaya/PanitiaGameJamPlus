using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Util;
using DG.Tweening;
using System;
using System.Threading;

public class ClickToMove : MonoBehaviour
{
    private CustomCoordinate coordinate;
    private MovementController movement;

    private Tween tween;

    private void Awake()
    {
        coordinate = GetComponent<CustomCoordinate>();
        movement = GetComponent<MovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mouse = MouseUtil.Get2DMousePosition();
            movement.GoToWorld(mouse).Forget();
            //Go(mouse).Forget();
            //DOTween.To(() => coordinate.position, pos => coordinate.position = pos, CustomCoordinate.WorldToGameCoordinate(mouse), 1).SetSpeedBased().SetEase(Ease.Linear);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Destroy(gameObject);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.Stop();
        }
    }

    async UniTask Go(Vector2 mouse)
    {
        if (tween != null)
        {
            tween.Kill();
        }
        tween = DOTween.To(() => coordinate.position, pos => coordinate.position = pos, CustomCoordinate.WorldToGameCoordinate(mouse), 1).SetSpeedBased().SetEase(Ease.Linear).SetLink(gameObject);
        await tween;
        if (tween.IsActive()) throw new OperationCanceledException();
        tween = null;
        Debug.Log("finish");
    }
}
