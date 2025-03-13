using System;
using System.Collections;
using System.Collections.Generic;

namespace Util
{
    public partial class StackUtil<T>
    {
        private Stack<T> stack;
        public Action<T> onPushEvent;
        public Action<T> onPopEvent;

        private bool isCheckMax;
        public int maxValue;

        public int Count => stack.Count;
        public bool IsMax => Count == maxValue;

        public StackUtil()
        {
            stack = new();
        }
        
        public StackUtil(int capacity, bool _isCheckMax)
        {
            stack = new(capacity);
            isCheckMax = _isCheckMax;
            maxValue = capacity;
        }
        
        public void Push(T item)
        {
            if(isCheckMax && Count == maxValue) return;
            stack.Push(item);
            onPushEvent?.Invoke(item);
        }

        public T Pop()
        {
            var item = stack.Pop();
            onPopEvent?.Invoke(item);
            return item;
        }

        public bool TryPop(out T item)
        {
            if (stack.TryPop(out item))
            {
                onPopEvent?.Invoke(item);
                return true;
            }

            return false;
        }

        public bool TryPeek(out T item)
        {
            return stack.TryPeek(out item);
        }
    }

    public partial class StackUtil<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in stack)
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