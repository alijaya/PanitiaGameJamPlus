using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class ClockUI : MonoBehaviour
{
    public float Progress
    {
        get
        {
            return _maxValue > 0 ? _value / _maxValue : 0;
        }
        set
        {
            _value = value * _maxValue;
            RefreshUI();
        }
    }

    [SerializeField]
    private float _value;

    public float Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            RefreshUI();
        }
    }

    [SerializeField]
    private float _maxValue;

    public float MaxValue
    {
        get
        {
            return _maxValue;
        }
        set
        {
            _maxValue = value;
            RefreshUI();
        }
    }

    public Transform needle;
    public UICircle arc;

    private void OnEnable()
    {
        RefreshUI();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += RefreshUI;
    }
#endif

    public void RefreshUI()
    {
        if (!this) return;

        if (needle)
        {
            var rotation = needle.localEulerAngles;
            rotation.z = 90 * (1 - Progress);
            needle.localEulerAngles = rotation;
        }
        if (arc) arc.SetProgress(Progress);
    }
}
