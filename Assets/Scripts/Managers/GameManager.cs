using Data;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private TickerQueue<string> _test;

        private void Awake()
        {
            print("Test Begins");
            
            _test = new TickerQueue<string>("Hallo", 10);
            _test.Enqueue("Welt", 11);
            
            print(_test.IsEmpty());
            
            _test.ToFirst();
            print(_test.HasAccess());
            
            while (_test.HasAccess())
            {
                print(_test.Dequeue() is null);
                _test.Next();
            }
        }
    }
}