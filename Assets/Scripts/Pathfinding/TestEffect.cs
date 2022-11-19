using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestEffect : MonoBehaviour
{
    public Transform[] effect;
    Vector3 worldPosition;
    int cycle = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
         {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
            Instantiate(effect[cycle], worldPosition, Quaternion.identity);
            cycle++;
            if(cycle == effect.Length) cycle = 0;
         }
    }
}
