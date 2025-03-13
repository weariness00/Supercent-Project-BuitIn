using System;
using System.Collections;
using Customer;
using Game.Dining;
using Game.Money;
using Manager;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Game
{
    public class Counter : MonoBehaviour
    {
        [Tooltip("계산 사운드")] public AudioClip calculateSound;
        
        [Header("Take Out 관련")]
        [Tooltip("포장 계산을 기다리는 장소")]public Transform takeOutWaitTransform;
        public QueueUtil<CustomerBase> hasTakeOutCustomerQueue = new(5, true);
        [Tooltip("포장지 프리펩")] public PaperBag takeOutBagPrefab;
        [Tooltip("포장지 생성 위치")] public Transform takeOutSpawnTransform;
        private bool isCalculateTakeOut = false; // 현재 Take Out 계산을 하고 있는지

        [Header("Dining 관련")] 
        public DiningRoom diningRoom;
        [Tooltip("매장 식사 계산을 기다리는 장소")]public Transform diningWaitTransform;
        public QueueUtil<CustomerBase> hasDiningCustomerQueue = new(5, true);

        [Header("Money Bundle 관련")] 
        public MoneyBundle moneyBundle;
            
        public void Awake()
        {
            hasTakeOutCustomerQueue.onEnqueueEvent += customer =>
            {
                MoveToLine(customer, takeOutWaitTransform, hasTakeOutCustomerQueue.Count - 1);
            };
            hasTakeOutCustomerQueue.onDequeueEvent += customer =>
            {
                int i = 0;
                foreach (var c in hasTakeOutCustomerQueue)
                {
                    MoveToCounter(c, takeOutWaitTransform, i++);
                }
            };
            
            hasDiningCustomerQueue.onEnqueueEvent += customer =>
            {
                MoveToLine(customer, diningWaitTransform, hasDiningCustomerQueue.Count - 1);
            };
            hasDiningCustomerQueue.onDequeueEvent += customer =>
            {
                int i = 0;
                foreach (var c in hasDiningCustomerQueue)
                {
                    MoveToCounter(c, takeOutWaitTransform, i++);
                }
            };
        }

        public void MoveToCounter(CustomerBase customer, Transform waitTransform, int index)
        {
            customer.agent.SetDestination(waitTransform.position + index * customer.agent.radius * waitTransform.forward);
        }

        private void MoveToLine(CustomerBase customer, Transform waitTransform, int index)
        {
            customer.agent.SetPath(
                new[]{transform.forward * 5f, waitTransform.position + index * customer.skinnedMeshRenderer.bounds.extents.z * waitTransform.forward},
                () =>
                {
                    if (index == 0 && customer.type == CustomerType.Dining)
                    {
                        customer.ui.ChangeDiningState();
                        diningRoom.CheckInCustomer(customer, () => hasDiningCustomerQueue.Dequeue());
                    }
                });
        }
        
        public void Calculate()
        {
            if (!isCalculateTakeOut && hasTakeOutCustomerQueue.TryPeek(out var customer) && customer.agent.velocity.magnitude < 0.01f)
            {
                StartCoroutine(TakeOut(customer));
                customer.ui.ChangeDisable();
            }
        }

        private IEnumerator TakeOut(CustomerBase customer)
        {
            isCalculateTakeOut = true;
            var bag = Instantiate(takeOutBagPrefab, takeOutSpawnTransform.position, takeOutSpawnTransform.rotation);
            var moneyAmount = 0;
            while (customer.hasBreadStack.TryPop(out var bread))
            {
                moneyAmount += bread.cellMoney;
                yield return bread.BreadMoveEnumerator(bag.transform, Vector3.zero, b =>
                {
                    Destroy(b.gameObject);
                }, null);
            }

            bag.SetClose();
            while (!bag.isClose)
                yield return null;
            yield return bag.PaperBagMoveEnumerator(customer.stackTransform, Vector3.up * 90f, null);
            hasTakeOutCustomerQueue.Dequeue();
            customer.OutShop();
            customer.GetBag(bag);

            var effectAudio = SoundManager.Instance.GetEffectSource();
            effectAudio.PlayOneShot(calculateSound);
            
            var moneySpawnCount = moneyAmount / 3 + 1;
            var moneyInterval = 3;
            for (int i = 0; i < moneySpawnCount; i++)
            {
                int addedAmount = Math.Min(moneyInterval, moneyAmount);
                moneyAmount -= addedAmount;
                moneyBundle.InstantiateMoney(addedAmount);
            }
            isCalculateTakeOut = false;
        }
    }
}

