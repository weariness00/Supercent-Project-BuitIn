using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Util;

namespace Customer
{
    public class CustomerContainer : MonoBehaviour
    {
        private HashSet<CustomerBase> customerHashSet = new();
        [SerializeField] private MinMaxValue<int> hasCustomerCount = new (0,0,10);
        [HideInInspector] public int totalCustomer = 0; // 이 컨테이너를 지나간 총 Customer의 수
        
        public UnityEvent<CustomerBase> onAddCustomerEvent = new();
        public UnityEvent<CustomerBase> onRemoveCustomerEvent = new();

        public bool IsHas => customerHashSet.Count != 0;
        
        public MinMaxValue<int> Count
        {
            get => hasCustomerCount;
            set => hasCustomerCount = value;
        }
        
        public CustomerBase AnyCustomer
        {
            get
            {
                foreach (var c in customerHashSet)
                    return c;
                return null;
            }
        }

        public bool TryAddCustomer(CustomerBase customer)
        {
            if (hasCustomerCount.IsMax && !hasCustomerCount.isOverMax) return false;
            customerHashSet.Add(customer);
            onAddCustomerEvent?.Invoke(customer);
            hasCustomerCount.Current++;
            totalCustomer++;
            return true;
        }
        
        public bool TryRemoveCustomer(CustomerBase customer)
        {
            if (hasCustomerCount.IsMin) return false;
            customerHashSet.Remove(customer);
            onRemoveCustomerEvent?.Invoke(customer);
            hasCustomerCount.Current--;
            return true;
        }
    }
}

