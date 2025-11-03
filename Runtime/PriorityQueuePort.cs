#nullable enable
using System;
using System.Collections.Generic;

namespace Caskev.GridToolkit.PriorityQueue
{
    /// <summary>
    /// Minimal priority queue (min-heap).
    /// </summary>
    internal sealed class PriorityQueue<TElement, TPriority>
    {
        private (TElement Item, TPriority Key)[] _heap;
        private int _count;
        private readonly IComparer<TPriority> _cmp;

        internal PriorityQueue() : this(0, comparer: null) { }

        internal PriorityQueue(int capacity, IComparer<TPriority>? comparer = null)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            _heap = capacity == 0 ? Array.Empty<(TElement, TPriority)>() : new (TElement, TPriority)[capacity];
            _cmp = comparer ?? Comparer<TPriority>.Default;
            _count = 0;
        }

        internal int Count => _count;

        internal void Clear()
        {
            if (_count == 0) return;
            Array.Clear(_heap, 0, _count);
            _count = 0;
        }

        internal void Enqueue(TElement element, TPriority priority)
        {
            if (_count == _heap.Length)
            {
                int newCap = _heap.Length == 0 ? 4 : _heap.Length * 2;
                Array.Resize(ref _heap, newCap);
            }
            SiftUp(_count++, (element, priority));
        }

        internal TElement Dequeue()
        {
            if (_count == 0) throw new InvalidOperationException("The queue is empty.");
            var rootItem = _heap[0].Item;
            var last = _heap[--_count];
            if (_count > 0) SiftDown(0, last);
            _heap[_count] = default;
            return rootItem;
        }

        private void SiftUp(int idx, (TElement Item, TPriority Key) node)
        {
            var heap = _heap; var cmp = _cmp;
            while (idx > 0)
            {
                int parent = (idx - 1) >> 1;
                if (cmp.Compare(node.Key, heap[parent].Key) >= 0) break;
                heap[idx] = heap[parent];
                idx = parent;
            }
            heap[idx] = node;
        }

        private void SiftDown(int idx, (TElement Item, TPriority Key) node)
        {
            var heap = _heap; var cmp = _cmp; int count = _count;
            while (true)
            {
                int left = (idx << 1) + 1;
                if (left >= count) break;
                int right = left + 1;
                int smallest = (right < count && cmp.Compare(heap[right].Key, heap[left].Key) < 0) ? right : left;
                if (cmp.Compare(node.Key, heap[smallest].Key) <= 0) break;
                heap[idx] = heap[smallest];
                idx = smallest;
            }
            heap[idx] = node;
        }
    }
}
