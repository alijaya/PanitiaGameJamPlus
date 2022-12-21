using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class DetectInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Keyboard.current.onTextInput += (char obj) => Debug.Log(obj); // this is the best
        //InputSystem.onAnyButtonPress.Call(OnButtonPress);
    }

    //void OnButtonPress(InputControl ctrl)
    //{
    //    Debug.Log(ctrl.name);
    //}

    // Update is called once per frame
    void Update()
    {
        //if (Input.inputString != "") Debug.Log(Input.inputString);
    }
}
