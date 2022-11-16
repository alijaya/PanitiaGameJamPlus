using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityAtoms.BaseAtoms;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ShowStringTMP : MonoBehaviour
{
    private TextMeshProUGUI text;

    public StringReference value;
    private StringEvent valueChanged;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        valueChanged = value.GetEvent<StringEvent>();
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
        text.text = value.Value;
    }
}
