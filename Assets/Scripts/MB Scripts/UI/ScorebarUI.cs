using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

[SelectionBase]
public class ScorebarUI : MonoBehaviour
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
    private List<int> _goals;

    public List<int> Goals
    {
        get
        {
            return _goals;
        }
        set
        {
            _goals = value;
            RefreshUI();
        }
    }

    public TMP_Text textUI;

    public RectTransform fillObject;

    public List<ScorebarGoalUI> goalUIs = new();

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

        if (textUI) textUI.text = "$ " + _value;

        if (fillObject)
        {
            var anchorMax = fillObject.anchorMax;
            anchorMax.x = Progress;

            fillObject.anchorMax = anchorMax;
        }

        for (var i = 0; i < goalUIs.Count; i++)
        {
            if (i >= _goals.Count) continue;

            var goal = _goals[i];
            var goalUI = goalUIs[i];
            if (!goalUI) continue;

            goalUI.MaxGoal = _maxGoal;
            goalUI.Value = goal;
            goalUI.Achieved = _value >= goal;
        }
    }
}
