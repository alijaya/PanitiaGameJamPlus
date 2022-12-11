using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Util;

public class ClickToMove : MonoBehaviour
{
    private PathFinder movement;

    private void Awake()
    {
        movement = GetComponent<PathFinder>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mouse = MouseUtil.Get2DMousePosition();
            movement.GoToWorld(mouse).Forget();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Destroy(gameObject);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.Stop();
        }
    }
}
