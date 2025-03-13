using System;
using UnityEngine;

namespace Player
{
    public class PlayerCameraControl : MonoBehaviour
    {
        public Camera camera;
        public Vector3 cameraOffset;
        public Quaternion cameraRotate;

        public void FixedUpdate()
        {
            if (!ReferenceEquals(camera, null))
            {
                TopView();
            }
        }

        public void TopView()
        {
            camera.transform.position = cameraOffset + transform.position;
            camera.transform.rotation = cameraRotate;
        }
    }
}

