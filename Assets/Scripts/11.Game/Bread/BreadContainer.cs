using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Util;

namespace Game.Bread
{
    public partial class BreadContainer : MonoBehaviour
    {
        [Tooltip("빵을 얻거나 내려놓을때 딜레이")]public MinMaxValue<float> delayAddOrRemove = new(0,0,0.1f); // 다른 객체에게 빵을 주거나 받을때 딜레이
        [Tooltip("빵이 저장될 공간")] public HashSet<BreadBase> hasBreadStack = new();
        [Tooltip("빵 갯수 정보")] [SerializeField] private MinMaxValue<int> hasBreadCount = new (0,0,10, false, true);
        public int totalBread = 0;

        public UnityEvent<BreadBase> onAddEvent = new();
        public UnityEvent<BreadBase> onRemoveEvent = new();

        public bool HasBread => hasBreadStack.Count != 0;
        public MinMaxValue<int> Count
        {
            get => hasBreadCount;
            set => hasBreadCount = value;
        }

        public void Awake()
        {
            hasBreadCount.isOverMax = true;
        }

        public void Update()
        {
            delayAddOrRemove.Current += Time.deltaTime;
        }

        public void Add(BreadBase bread)
        {
            hasBreadStack.Add(bread);
            delayAddOrRemove.SetMin();
            hasBreadCount.Current++;
            totalBread++;

            onAddEvent?.Invoke(bread);
        }

        public bool TryAdd(BreadBase bread)
        {
            if(!delayAddOrRemove.IsMax || hasBreadCount.IsMax) return false;
            
            hasBreadStack.Add(bread);
            delayAddOrRemove.SetMin();
            hasBreadCount.Current++;
            totalBread++;

            onAddEvent?.Invoke(bread);

            return true;
        }

        public void Remove(BreadBase bread)
        {
            hasBreadStack.Remove(bread);
            delayAddOrRemove.SetMin();
            hasBreadCount.Current--;
            
            onRemoveEvent?.Invoke(bread);
        }
        
        public bool TryRemove(BreadBase bread)
        {
            if(!delayAddOrRemove.IsMax) return false;

            hasBreadStack.Remove(bread);
            delayAddOrRemove.SetMin();
            hasBreadCount.Current--;
            
            onRemoveEvent?.Invoke(bread);
            
            return true;
        }

        public void Clear()
        {
            hasBreadStack.Clear();
            hasBreadCount.SetMin();
        }
    }

    public partial class BreadContainer : IEnumerable<BreadBase>
    {
        public IEnumerator<BreadBase> GetEnumerator()
        {
            foreach (var item in hasBreadStack)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

