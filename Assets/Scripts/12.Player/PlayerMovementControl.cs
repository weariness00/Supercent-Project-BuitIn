using System;
using UnityEngine;

namespace Player
{
    public class PlayerMovementControl : MonoBehaviour
    {
        public FloatingJoystick joystick;
        public float speed = 3;

        [HideInInspector] public bool isMove = false;
        public void FixedUpdate()
        {
            var direction = Vector3.forward * joystick.Vertical + Vector3.right * joystick.Horizontal;
            isMove = direction.magnitude != 0;

            Move(direction);
            Rotate(direction);
        }

        public void Move(Vector3 _direction)
        {
            transform.position += Time.fixedDeltaTime * speed * _direction;
        }

        public void Rotate(Vector3 _direction)
        {
            transform.LookAt(transform.position + _direction);
        }
    }
}

