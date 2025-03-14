using System.Collections;
using Customer;
using Game.Bread;
using UnityEngine;
using Util;

namespace Game
{
    public partial class DisplayStand : MonoBehaviour
    {
        public StackUtil<BreadBase> hasBreadStack = new(10, true);
        public Transform[] breadStandTransformArray;

        public MinMaxValue<int> hasCustomerCount = new (0,0,10);
        public float customerWaitRadius = 1f;

        [HideInInspector] public int customerCount = 0;

        public void Awake()
        {
            hasBreadStack.onPushEvent += bread =>
            {
                bread.PutBreadMove(breadStandTransformArray[hasBreadStack.Count - 1], Vector3.zero, null);
            };
        }

        public bool TryAddCustomer(CustomerBase customer)
        {
            if (hasCustomerCount.IsMax) return false;
            hasCustomerCount.Current++;
            customerCount++;
            customer.agent.SetDestination(GetAroundPosition());
            
            return true;
        }

        public bool TryPopCustomer(CustomerBase customer)
        {
            if (hasCustomerCount.IsMin) return false;
            hasCustomerCount.Current--;

            return true;
        }

        public Vector3 GetAroundPosition()
        {
            // 360도라고 할때
            // hasBreadStack 길이만큼 나누어서 배분
            // 구형이 아닌 box형에 가깝게 해주어야함

            float interval = 360f / hasCustomerCount.Max;
            float delta = (customerCount % hasCustomerCount.Max) * interval;
            float angleInRadians = delta * Mathf.Deg2Rad; // 각도를 라디안으로 변환
            float x = customerWaitRadius * Mathf.Sin(angleInRadians); // x 좌표
            float z = customerWaitRadius * Mathf.Cos(angleInRadians); // z 좌표
            x = Mathf.Clamp(x, -customerWaitRadius, customerWaitRadius);
            z = Mathf.Clamp(z, -customerWaitRadius, customerWaitRadius);

            var pos = transform.position + new Vector3(x, 0, z);
            return pos;
        }
    }
    
    public partial class DisplayStand : IArea
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

