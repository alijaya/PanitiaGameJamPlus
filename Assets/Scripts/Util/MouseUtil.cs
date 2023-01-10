using UnityEngine;

namespace Util {
    public class MouseUtil {
        public static Vector2 Get2DMousePosition() {
            
            var mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            
            var worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
            return worldPosition;
        }
    }
}
