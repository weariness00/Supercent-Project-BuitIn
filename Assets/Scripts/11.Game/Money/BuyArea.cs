using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using User;
using Util;

namespace Game.Money
{
    public class BuyArea : MonoBehaviour
    {
        [Tooltip("필요한 돈")] public MinMaxValue<int> needMoney = new(0, 0, 30);
        [Tooltip("돈 가져오는 모션 몇번 보여줄 것인지")] public int moneyMotionCount = 10;
        [Tooltip("돈 모션에 사용될 프리펩")] public MoneyBase moneyPrefab;

        [Header("2D Object 관련")] 
        public TMP_Text moneyAmountText;

        [Tooltip("돈이 충족되면 실행될 이벤트")] public UnityEvent onBuyEvent;

        private MinMaxValue<float> getMoneyTimer = new(0, 0, 0.01f);

        public void Awake()
        {
            needMoney.SetMax();
            moneyAmountText.text = needMoney.Max.ToString();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                getMoneyTimer.SetMin();
            }
        }

        public void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                getMoneyTimer.Current += Time.deltaTime;
                if (getMoneyTimer.IsMax)
                {
                    getMoneyTimer.SetMin();
                    var moneyAmount = UserManager.Instance.userData.money;
                    int addedMoney = Math.Min(needMoney.Max / moneyMotionCount, needMoney.Current);
                    if (moneyAmount.Value >= addedMoney)
                    {
                        moneyAmount.Value -= addedMoney;
                        needMoney.Current -= addedMoney;
                        moneyAmountText.text = needMoney.Current.ToString();
                        var money = Instantiate(moneyPrefab, other.transform.position, other.transform.rotation);
                        money.MoneyMove(transform, Vector3.zero);

                        Buy();  
                    }
                }
            }
        }

        private void Buy()
        {
            if (needMoney.IsMin)
            {
                Destroy(gameObject);
                onBuyEvent?.Invoke();
            }
        }
    }
}

