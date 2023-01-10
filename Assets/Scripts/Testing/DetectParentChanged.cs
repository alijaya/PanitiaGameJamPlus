using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DetectParentChanged : MonoBehaviour
{
    private void OnTransformParentChanged()
    {
        Debug.Log("Changed!");
    }
}
