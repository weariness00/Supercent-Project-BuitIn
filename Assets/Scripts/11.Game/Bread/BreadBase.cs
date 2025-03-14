using System;
using System.Collections;
using Manager;
using UnityEngine;
using Util;

namespace Game.Bread
{
    public class BreadBase : MonoBehaviour
    {
        [HideInInspector] public Rigidbody rigidbody;
        [HideInInspector] public Collider collider;
        [HideInInspector] public SkinnedMeshRenderer skinnedMeshRenderer;
        public int cellMoney = 6;

        public MinMaxValue<float> moveTimer = new(0,0,0.1f);
        [HideInInspector] public bool isMove = false;

        public AudioClip getSound;
        public AudioClip putSound;
        
        public void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            collider = GetComponent<Collider>();
        }

        public void GetBreadMove(Transform targetTransform,  Vector3 offset, Action<BreadBase> onMoveCompleteEvent = null)
        {
            if(isMove) return;
            StartCoroutine(BreadMoveEnumerator(targetTransform, offset, onMoveCompleteEvent, getSound));
        }
        
        public void PutBreadMove(Transform targetTransform,  Vector3 offset, Action<BreadBase> onMoveCompleteEvent = null)
        {
            if(isMove) return;
            StartCoroutine(BreadMoveEnumerator(targetTransform, offset, onMoveCompleteEvent, putSound));
        }
        
        public IEnumerator BreadMoveEnumerator(Transform targetTransform, Vector3 offset, Action<BreadBase> onMoveCompleteEvent, AudioClip sound)
        {
            isMove = true;
            
            var originPos = transform.position;
            transform.SetParent(targetTransform, false);
            transform.position = originPos;
            
            var startPos = transform.position;
            var startAngles = transform.eulerAngles;
            moveTimer.SetMin();
            while (!moveTimer.IsMax)
            {
                moveTimer.Current += Time.deltaTime;
                var t = moveTimer.Current / moveTimer.Max;
                var destPos = targetTransform.position + offset;
                transform.position = Vector3Extension.Cubic(startPos, startPos + Vector3.up, destPos + Vector3.up * 2f, destPos, t);
                transform.eulerAngles = Vector3.Lerp(startAngles, targetTransform.eulerAngles, t);
                yield return null;
            }
            transform.position = targetTransform.position + offset;
            transform.eulerAngles = targetTransform.eulerAngles;
            
            isMove = false;
            if (sound != null)
            {
                var effectAudio = SoundManager.Instance.GetEffectSource();
                effectAudio.PlayOneShot(sound);
            }
            onMoveCompleteEvent?.Invoke(this);
        }
    }
}

