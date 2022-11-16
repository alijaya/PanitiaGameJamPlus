using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityAtoms.BaseAtoms;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ShowIntTMP : MonoBehaviour
{
    private TextMeshProUGUI text;

    public IntReference value;
    private IntEvent valueChanged;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        valueChanged = value.GetEvent<IntEvent>();
    }

    private void OnEnable()
    {
        valueChanged.Register(UpdateDisplay);
        UpdateDisplay();
    }

    private void OnDisable()
    {
        valueChanged.Unregister(UpdateDisplay);
    }

    public void UpdateDisplay()
    {
        text.text = value.Value.ToString();
    }
}
