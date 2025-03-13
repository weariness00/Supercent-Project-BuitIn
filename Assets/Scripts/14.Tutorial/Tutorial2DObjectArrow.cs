using System;
using UnityEngine;

namespace Tutorial
{
    public class Tutorial2DObjectArrow : MonoBehaviour
    {
        [HideInInspector] public Transform destTransform;
        [HideInInspector] public Transform ownerTransform;

        [HideInInspector] public bool isAroundMoveToDest;
        
        public void LateUpdate()
        {
            if (isAroundMoveToDest)
            {
                var ownerPos = ownerTransform.position;
                var destPos = destTransform.position;
                var dir = Vector3.Normalize(destPos - ownerPos);
                dir.y = 0;
                transform.position = ownerPos + dir + Vector3.up * 0.1f;
                transform.LookAt(destTransform);
                transform.Rotate(90,0,0);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void SetAroundMoveToDestination(Transform _destTransform, Transform _ownerTransform)
        {
            isAroundMoveToDest = true;
            destTransform = _destTransform;
            ownerTransform = _ownerTransform;
            gameObject.SetActive(true);
        }
    }
}

