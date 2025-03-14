using System;
using System.Collections;
using Customer;
using Game.Dining;
using Game.Money;
using Manager;
using UnityEngine;
using Util;

namespace Game
{
    public partial class Counter : MonoBehaviour
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
                MoveToCounter(customer, takeOutWaitTransform, hasTakeOutCustomerQueue.Count - 1);
            };
            hasTakeOutCustomerQueue.onDequeueEvent += customer =>
            {
                int i = 0;
                foreach (var c in hasTakeOutCustomerQueue)
                {
                    MoveToLine(c, takeOutWaitTransform, i++);
                }
            };
            
            hasDiningCustomerQueue.onEnqueueEvent += customer =>
            {
                MoveToCounter(customer, diningWaitTransform, hasDiningCustomerQueue.Count - 1);
            };
            hasDiningCustomerQueue.onDequeueEvent += customer =>
            {
                int i = 0;
                foreach (var c in hasDiningCustomerQueue)
                {
                    MoveToLine(c, diningWaitTransform, i++);
                }
            };
        }

        public void MoveToCounter(CustomerBase customer, Transform waitTransform, int index)
        {
            var pos1 = transform.forward * 5f;
            var pos2 = waitTransform.position + index * customer.skinnedMeshRenderer.bounds.extents.z * waitTransform.forward;
            customer.agent.SetPath(new[]{pos1, pos2}, 
            () =>
            {
                if (index == 0 && customer.type == CustomerType.Dining)
                {
                    customer.ui.ChangeDiningState();
                    diningRoom.CheckInCustomer(customer, () => hasDiningCustomerQueue.Dequeue());
                }
            });
        }

        private void MoveToLine(CustomerBase customer, Transform waitTransform, int index)
        {
            var pos = waitTransform.position + index * customer.skinnedMeshRenderer.bounds.extents.z * waitTransform.forward;
            customer.agent.SetDestination(pos, 
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
            
            moneyBundle.InstantiateMoneyRange(moneyAmount, 2);
            isCalculateTakeOut = false;
        }
    }
    public partial class Counter : IArea
    {
        [Header("Area 관련")]
        [SerializeField] private GameObject area2DObject;
        public GameObject Area2DObject { get; set; }
        public void AreaEnter()
        {
            area2DObject.transform.localScale = Vector3.one * 1.1f;
        }

        public void AreaExit()
        {
            area2DObject.transform.localScale = Vector3.one;
        }
    }
}

