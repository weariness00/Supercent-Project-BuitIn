using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial class PaperBag : MonoBehaviour
    {
        [HideInInspector] public bool isClose;
        
        public void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }
        
        public IEnumerator PaperBagMoveEnumerator(Transform targetTransform, Vector3 angleOffset, Action<PaperBag> onMoveCompleteEvent)
        {
            var originPos = transform.position;
            transform.SetParent(targetTransform, false);
            transform.position = originPos;
            
            float et = 0;
            var duration = 0.5f;
            var startPos = transform.position;
            var startAngles = transform.eulerAngles;
            var destPos = targetTransform.position;
            var destAngles = targetTransform.eulerAngles + angleOffset;
            while (et <= duration)
            {
                et += Time.deltaTime;
                var t = et / duration;
                var newPos = Vector3.Lerp(startPos, destPos, t);
                transform.position = newPos;
                transform.eulerAngles = Vector3.Lerp(startAngles, destAngles, et / duration);
                yield return null;
            }

            transform.position = destPos;
            transform.eulerAngles = destAngles;
            
            onMoveCompleteEvent?.Invoke(this);
        }
    }

    public partial class PaperBag
    {
        [HideInInspector] public Animator animator;
        private static readonly int GoClose = Animator.StringToHash("Go Close");
        public void SetClose() => animator.SetTrigger(GoClose);

        public void OnCompleteCloseAnimation()
        {
            isClose = true;
        }
    }
}

