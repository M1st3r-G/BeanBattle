using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public class TickerQueue<TItem>
    {
        private Dictionary<TItem, int> _intern = new();
        
        public void Enqueue(TItem pItem, int pPriority)
        {
            _intern[pItem] = pPriority;
        }

        public void ManualCleanUp()
        {
            var max = _intern.Aggregate((l, r) => l.Value < r.Value ? l : r).Value;
            if (max != 0)
            {
                LowerAllBy(max);
            }
        }
        
        public TItem Dequeue()
        {
            var max = _intern.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;
            var val = _intern[max];
            _intern.Remove(max);
            LowerAllBy(val);
            return max;
        }

        public KeyValuePair<TItem, int>[] GetCurrentState()
        {
            return (from entry in _intern orderby entry.Value ascending select entry).ToArray();
        }
        
        public bool IsEmpty()
        {
            return _intern.Count == 0;
        }
        
        private void LowerAllBy(int amount)
        {
            if (amount == 0) return;
            _intern = _intern.ToDictionary(p => p.Key, p => p.Value - amount);
        }
    }
}