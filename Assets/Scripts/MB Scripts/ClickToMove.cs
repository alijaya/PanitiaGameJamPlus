using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ClickToMove : MonoBehaviour
{
    private PathFinder2 movement;

    private void Awake()
    {
        movement = GetComponent<PathFinder2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            movement.GoTo(Util.Get2DMousePosition()).Forget();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Destroy(gameObject);
        }
    }
}
