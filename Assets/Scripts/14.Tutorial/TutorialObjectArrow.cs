using System;
using DG.Tweening;
using UnityEngine;

namespace Tutorial
{
    public class TutorialObjectArrow : MonoBehaviour
    {
        public void Start()
        {
            // 객체를 Y축으로 튕기게 설정
            transform.DOMoveY(transform.position.y + 1f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine); 
        }

        public void SetTutorialTarget(Transform _targetTransform)
        {
            transform.position = _targetTransform.position + Vector3.up * 2f;
        }
    }
}

