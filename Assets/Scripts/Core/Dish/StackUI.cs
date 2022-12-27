using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StackUI : MonoBehaviour
{
    public TMP_Text text;

    public void SetValue(int value)
    {
        text.text = value.ToString();
    }

    public void SetValue(string value)
    {
        text.text = value;
    }
}
