using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomCoordinate))]
public class CustomCoordinateEditor : Editor
{
    private CustomCoordinate customCoordinate;

    private Transform transform;
    private Vector3 pos;

    private Vector3 xAxis;
    private Vector3 yAxis;
    private Vector3 zAxis;

    private void OnEnable()
    {
        Tools.hidden = true;
    }

    private void OnDisable()
    {
        Tools.hidden = false;
    }

    public void OnSceneGUI()
    {
        customCoordinate = target as CustomCoordinate;
        transform = customCoordinate.transform;
        pos = transform.position;
        xAxis = CustomCoordinate.axisX;
        yAxis = CustomCoordinate.axisY;
        zAxis = CustomCoordinate.axisZ;
        var tempColor = Handles.color;
        DrawPositionHandles();
        Handles.color = tempColor;
    }

    public void DrawPositionHandles()
    {
        var recordObjects = new Object[] { customCoordinate, transform };

        Handles.color = Handles.xAxisColor;
        EditorGUI.BeginChangeCheck();
        var newPosX = Handles.Slider(pos, xAxis);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(recordObjects, "Change CustomCoordinate X");
            customCoordinate.localPosition += Vector3.right * Vector3.Dot(newPosX - pos, xAxis);
        }

        Handles.color = Handles.yAxisColor;
        EditorGUI.BeginChangeCheck();
        var newPosY = Handles.Slider(pos, yAxis);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(recordObjects, "Change CustomCoordinate Y");
            customCoordinate.localPosition += Vector3.up * Vector3.Dot(newPosY - pos, yAxis);
        }

        Handles.color = Handles.zAxisColor;
        EditorGUI.BeginChangeCheck();
        var newPosZ = Handles.Slider(pos, zAxis);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(recordObjects, "Change CustomCoordinate Z");
            customCoordinate.localPosition += Vector3.forward * Vector3.Dot(newPosZ - pos, zAxis);
        }
    }

}