using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

[ExecuteAlways]
public class CustomCoordinate : MonoBehaviour
{
    public readonly static Vector3 axisX = new Vector3(1, 0, 0);
    public readonly static Vector3 axisY = new Vector3(0, 0.1f, 0);
    public readonly static Vector3 axisZ = new Vector3(0, Mathf.Sqrt(3) / 2, -Mathf.Sqrt(3) / 2);
    public readonly static Matrix4x4 matTransform = new Matrix4x4(axisX, axisY, axisZ, new Vector4(0, 0, 0, 1));
    public readonly static Matrix4x4 matITransform = matTransform.inverse;

    public static Vector3 WorldToGameCoordinate(Vector3 pos)
    {
        return matITransform.MultiplyPoint3x4(pos);
    }

    public static Vector3 GameToWorldCoordinate(Vector3 pos)
    {
        return matTransform.MultiplyPoint3x4(pos);
    }

    private Vector3 _lastPosition;
    private CustomCoordinate _lastParent;

    [SerializeField]
    private Vector3 _lastLocalPosition;

    public Vector3 localPosition
    {
        get
        {
            return _lastLocalPosition;
        }
        set
        {
            _lastLocalPosition = value;
            OnPositionChanged();
        }
    }

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

    private void Update()
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
            }
            else
            {
                parentPositionChanged(Vector3.zero);
            }
            position = _lastPosition;
        }
        OnPositionChanged();
    }

    private void OnPositionChanged()
    {
        // If localPosition changed
        // Recalculate position
        // If parent position changed
        // Recalculate position

        var curPos = position; // cache
        transform.position = GameToWorldCoordinate(curPos);
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