using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(ScorebarUI))]
public class ScorebarSetScoreUI : MonoBehaviour
{
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

    public int MaxGoal
    {
        get
        {
            return _goals.Max();
        }
    }

    private ScorebarUI scorebar;

    private void Awake()
    {
        scorebar = GetComponent<ScorebarUI>();
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    private void OnValidate()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        scorebar.Progress = _value / MaxGoal;
    }
}
