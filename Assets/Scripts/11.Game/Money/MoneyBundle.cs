using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Money
{
    public class MoneyBundle : MonoBehaviour
    {
        public ObjectPool<MoneyBase> moneyPool;
        [HideInInspector] public List<MoneyBase> activeMoneyList = new();
        public MoneyBase moneyPrefab;

        public bool HasMoney => moneyPool.CountActive != 0;
        
        public void Awake()
        {
            moneyPool = new(
                () => Instantiate(moneyPrefab, transform),
                money =>
                {
                    money.gameObject.SetActive(true);
                    activeMoneyList.Add(money);
                },
                money =>
                {
                    money.gameObject.SetActive(false);
                    activeMoneyList.Remove(money);
                },
                money => Destroy(money.gameObject),
                false,
                30,
                30
            );
        }

        public void InstantiateMoney(int moneyAmount)
        {
            var money = moneyPool.Get();
            money.moneyAmount = moneyAmount;
        }
    }
}

