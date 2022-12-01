using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteAlways]
public class CustomCoordinate : MonoBehaviour
{
    public readonly static Matrix4x4 matTransform = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 0.5f, 0, 0), new Vector4(0, Mathf.Sqrt(3) / 2, -Mathf.Sqrt(3) / 2, 0), new Vector4(0, 0, 0, 1));

    private Vector3 _lastLocalPosition;
    private Vector3 _lastPosition;
    private CustomCoordinate _lastParent;
    private Vector3 _lastParentPosition;

    public Vector3 localPosition;

    public event Action<Vector3> positionChanged;

    public Vector3 parentPosition { get; private set; }

    public Vector3 position
    {
        get
        {
            return localPosition + parentPosition;
        }
        set
        {
            localPosition = value - parentPosition;
        }
    }

    // Update is called once per frame
    public void Update()
    {
        // If parent changed
        // Recalculate localPosition
        // If position changed
        // Recalculate localPosition
        var parent = transform.parent?.GetComponentInParent<CustomCoordinate>();
        if (_lastParent != parent)
        {
            if (_lastParent) _lastParent.positionChanged -= parentPositionChanged;

            _lastParent = parent;

            if (_lastParent)
            {
                _lastParent.positionChanged += parentPositionChanged;
                parentPositionChanged(_lastParent.position);
            } else
            {
                parentPositionChanged(Vector3.zero);
            }
            position = _lastPosition;
        }

        // If localPosition changed
        // Recalculate position
        // If parent position changed
        // Recalculate position

        var curPos = position; // cache
        transform.position = matTransform.MultiplyPoint3x4(curPos);
        if (_lastPosition != curPos)
        {
            _lastPosition = curPos;
            positionChanged?.Invoke(curPos); // notify child
        }
    }

    private void parentPositionChanged(Vector3 parentPosition)
    {
        this.parentPosition = parentPosition;
    }

    void OnDrawGizmos()
    {
        var tempColor = Gizmos.color;
        Gizmos.color = Color.yellow;
        var groundPos = position;
        groundPos.z = 0;
        var groundWorldPos = matTransform.MultiplyPoint3x4(groundPos);
        Gizmos.DrawLine(transform.position, groundWorldPos);
        Gizmos.color = tempColor;
    }
}