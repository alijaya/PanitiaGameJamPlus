using UnityEngine;

namespace Util {
    public class MouseUtil: SingletonMB<MouseUtil> {
        private static Camera _mainCamera;

        protected override void SingletonAwakened() {
            base.SingletonAwakened();
            _mainCamera = Camera.main;
        }

        public static Vector2 Get2DMousePosition() {
            
            var mousePos = Input.mousePosition;
            mousePos.z = _mainCamera.nearClipPlane;
            
            var worldPosition = _mainCamera.ScreenToWorldPoint(mousePos);
            return worldPosition;
        }
    }
}
