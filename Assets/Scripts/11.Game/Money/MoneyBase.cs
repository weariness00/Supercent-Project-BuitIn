using System.Collections;
using UnityEngine;
using User;
using Util;

namespace Game.Money
{
    public class MoneyBase : MonoBehaviour
    {
        public int moneyAmount = 3;
        public MinMaxValue<float> moveTimer = new(0,0,0.1f);

        public void MoneyMove(Transform targetTransform ,Vector3 offset)
        {
            StartCoroutine(MoneyMoveEnumerator(targetTransform, offset));
        }
        
        private IEnumerator MoneyMoveEnumerator(Transform targetTransform, Vector3 offset)
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

            Destroy(gameObject);
        }
    }
}

