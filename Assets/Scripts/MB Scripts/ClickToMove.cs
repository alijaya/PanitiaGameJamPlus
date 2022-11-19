using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    private MovementController movement;

    private void Awake()
    {
        movement = GetComponent<MovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            movement.GoTo(Util.Get2DMousePosition());
        }
    }
}
