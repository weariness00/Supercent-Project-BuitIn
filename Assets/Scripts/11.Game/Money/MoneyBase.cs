using System.Collections;
using Manager;
using UnityEngine;
using User;
using Util;

namespace Game.Money
{
    public class MoneyBase : MonoBehaviour
    {
        public int moneyAmount = 3;
        public MinMaxValue<float> moveTimer = new(0,0,0.1f);

        public AudioClip getSound;

        public void GetMoneyMove(Transform targetTransform ,Vector3 offset)
        {
            StartCoroutine(MoneyMoveEnumerator(targetTransform, offset, getSound));
        }
        
        public void PutMoneyMove(Transform targetTransform,Vector3 offset)
        {
            StartCoroutine(MoneyMoveEnumerator(targetTransform, offset, null));
        }
        
        private IEnumerator MoneyMoveEnumerator(Transform targetTransform, Vector3 offset, AudioClip sound)
        {
            var originPos = transform.position;
            transform.SetParent(targetTransform, false);
            transform.position = originPos;

            var up = Vector3.up * 3f;
            var startPos = transform.position;
            var startAngles = transform.eulerAngles;
            moveTimer.SetMin();
            while (!moveTimer.IsMax)
            {
                moveTimer.Current += Time.deltaTime;
                var t = moveTimer.Current / moveTimer.Max;
                var destPos = targetTransform.position + offset;
                transform.position = Vector3Extension.Cubic(startPos, startPos + up, destPos +up * 2f, destPos, t);
                transform.eulerAngles = Vector3.Lerp(startAngles, targetTransform.eulerAngles, t);
                yield return null;
            }
            transform.position = targetTransform.position;
            transform.eulerAngles = targetTransform.eulerAngles;

            if (sound)
            {
                var effectAudio = SoundManager.Instance.GetEffectSource();
                effectAudio.pitch = 2f;
                effectAudio.PlayOneShot(sound);
                effectAudio.pitch = 1f;
            }
            
            Destroy(gameObject);
        }
    }
}

