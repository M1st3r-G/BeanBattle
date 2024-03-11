using System;

namespace Data
{
    public class TickerQueue<TItem>
    {
        private TickerQueueNode _first;
        private TickerQueueNode _last;
        private TickerQueueNode _current;
        
        public TickerQueue(TItem pItem, int pPriority)
        {
            _first = _last = new TickerQueueNode(pItem, pPriority);
            Debug();
        }

        public void Debug()
        {
            Console.Out.WriteLine($"f: {_first}|l: {_last}");
        }
        
        public void Enqueue(TItem pItem, int pPriority)
        {
            if (IsEmpty())
            {
                _first = _last = new TickerQueueNode(pItem, pPriority);
            }
            else if (_first.Priority > pPriority)
            {
                var newNode = new TickerQueueNode(pItem, pPriority)
                {
                    NextNode = _first
                };
                _first = newNode;
            }
            else
            {
                TickerQueueNode newNode = new TickerQueueNode(pItem, pPriority);
                ToFirst();
                while (HasAccess())
                {
                    if (_current.Priority > pPriority)
                    {
                        newNode.NextNode = _current;
                        PreviousNode(_current).NextNode = newNode;
                        Debug();
                        return;
                    }
                    Next();
                }

                _last.NextNode = newNode;
                _last = newNode;
            }

            Debug();
        }

        public TItem Dequeue()
        {
            if (IsEmpty()) return default;
            TickerQueueNode tmp = _first;
            if (_first == _last)
            {
                _first = _last = null;
                return tmp.Item;
            }

            _first = _first.NextNode;
            LowerAllBy(tmp.Priority);
            return tmp.Item;
        }

        private void LowerAllBy(int amount)
        {
            ToFirst();
            while (HasAccess())
            {
                _current.Priority -= amount;
                Next();
            }
        }
        
        private TickerQueueNode PreviousNode(TickerQueueNode pNode)
        {
            var tmp = _first;
            while (tmp is not null)
            {
                if (tmp.NextNode == pNode) return tmp;
                tmp = tmp.NextNode;
            }

            return null;
        }
        
        public void ToFirst()
        {
            _current = _first;
        }
        
        public bool HasAccess()
        {
            return _current is null;
        }

        public void Next()
        {
            _current = _current.NextNode;
        }
        
        public bool IsEmpty()
        {
            return _first is null;
        }
        
        private class TickerQueueNode
        {
            public readonly TItem Item;
            public int Priority;

            public TickerQueueNode NextNode;

            public TickerQueueNode(TItem pItem, int pPriority)
            {
                Item = pItem;
                Priority = pPriority;
            }
        }
    }
}