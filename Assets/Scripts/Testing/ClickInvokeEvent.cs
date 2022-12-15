using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickInvokeEvent : MonoBehaviour
{
    public UnityEvent OnClick = new ();

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            OnClick.Invoke();
        }
    }
}
