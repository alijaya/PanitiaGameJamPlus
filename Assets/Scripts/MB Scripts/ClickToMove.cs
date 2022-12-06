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
            var mouse = Util.Get2DMousePosition();
            Debug.Log(mouse);
            movement.GoToWorld(mouse).Forget();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Destroy(gameObject);
        }
    }
}
