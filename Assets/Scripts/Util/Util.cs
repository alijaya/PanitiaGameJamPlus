using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static Vector2 Get2DMousePosition()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        var worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        return worldPosition;
    }
}
