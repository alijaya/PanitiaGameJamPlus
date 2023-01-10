using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugValue : MonoBehaviour
{
    public TMP_Text text;

    public StringFunc stringFunc;
    public IntFunc intFunc;
    public FloatFunc floatFunc;
    public BoolFunc boolFunc;

    public string prefix = "";
    public string suffix = "";
    public int decimalPlaces = 2;
    public string trueValue = "true";
    public string falseValue = "false";

    // Update is called once per frame
    void Update()
    {
        if (stringFunc.target != null)
        {
            text.text = prefix + stringFunc.Invoke() + suffix;
        }
        else if (intFunc.target != null)
        {
            text.text = prefix + intFunc.Invoke().ToString("n0") + suffix;
        }
        else if (floatFunc.target != null)
        {
            text.text = prefix + floatFunc.Invoke().ToString("n"+decimalPlaces) + suffix;
        } else if (boolFunc.target != null)
        {
            text.text = prefix + (boolFunc.Invoke() ? trueValue : falseValue) + suffix;
        }
    }
}
