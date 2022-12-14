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

    public int decimalPlaces = 2;
    public string trueValue = "true";
    public string falseValue = "false";

    // Update is called once per frame
    void Update()
    {
        if (stringFunc.target != null)
        {
            text.text = stringFunc.Invoke();
        }
        else if (intFunc.target != null)
        {
            text.text = intFunc.Invoke().ToString("n0");
        }
        else if (floatFunc.target != null)
        {
            text.text = floatFunc.Invoke().ToString("n"+decimalPlaces);
        } else if (boolFunc.target != null)
        {
            text.text = boolFunc.Invoke() ? trueValue : falseValue;
        }
    }
}
