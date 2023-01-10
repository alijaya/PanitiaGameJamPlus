using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScorebarGoalUI : MonoBehaviour
{
    public float Progress
    {
        get
        {
            return _maxGoal > 0 ? (float) _value / _maxGoal : 0;
        }
        set
        {
            _value = Mathf.FloorToInt(value * _maxGoal);
            RefreshUI();
        }
    }

    [SerializeField]
    private int _value;

    public int Value
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
    private int _maxGoal;

    public int MaxGoal
    {
        get
        {
            return _maxGoal;
        }
        set
        {
            _maxGoal = value;
            RefreshUI();
        }
    }

    [SerializeField]
    private bool _achieved;

    public bool Achieved
    {
        get
        {
            return _achieved;
        }
        set
        {
            _achieved = value;
            RefreshUI();
        }
    }

    public List<SpriteState> icons = new ();
    public int unachievedState = 0;
    public int achievedState = 1;

    public CanvasGroup indicator;
    public float unachievedFade = 1;
    public float achievedFade = .5f;
    public TMP_Text textUI;

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

        var rect = GetComponent<RectTransform>();
        var min = rect.anchorMin;
        min.x = Progress;
        rect.anchorMin = min;

        var max = rect.anchorMax;
        max.x = Progress;
        rect.anchorMax = max;

        foreach (var icon in icons)
        {
            if (!icon) continue;

            icon.State = _achieved ? achievedState : unachievedState;
        }

        if (indicator) indicator.alpha = _achieved ? achievedFade : unachievedFade;

        if (textUI) textUI.text = "$ " + _value;
    }
}
