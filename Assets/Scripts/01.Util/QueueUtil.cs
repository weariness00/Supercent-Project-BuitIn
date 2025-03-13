using System;
using System.Collections;
using System.Collections.Generic;

namespace Util
{
    public partial class QueueUtil<T>
    {
        private Queue<T> queue;
        
        public Action<T> onEnqueueEvent;
        public Action<T> onDequeueEvent;

        private bool isCheckMax;
        public int maxValue;

        public int Count => queue.Count;
        public bool IsMax => Count == maxValue;

        public QueueUtil()
        {
            queue = new();
        }
        
        public QueueUtil(int capacity, bool _isCheckMax)
        {
            queue = new(capacity);
            isCheckMax = _isCheckMax;
            maxValue = capacity;
        }
        
        public void Enqueue(T item)
        {
            if(isCheckMax && Count == maxValue) return;
            queue.Enqueue(item);
            onEnqueueEvent?.Invoke(item);
        }

        public T Dequeue()
        {
            var item = queue.Dequeue();
            onDequeueEvent?.Invoke(item);
            return item;
        }

        public bool TryDequeue(out T item)
        {
            if (queue.TryDequeue(out item))
            {
                onDequeueEvent?.Invoke(item);
                return true;
            }

            return false;
        }

        public bool TryPeek(out T item)
        {
            return queue.TryPeek(out item);
        }
    }

    public partial class QueueUtil<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in queue)
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