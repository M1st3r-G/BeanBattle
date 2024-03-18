using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputActionReference nextPhaseAction;
        private List<CharController> _playOrder;
        private CharController _current;
        private bool _nextPhasePressed;
        
        public delegate void OnOrderChangeDelegate(CharController[] newOrder);
        public static OnOrderChangeDelegate OnOrderChanged;
        public delegate void OnCurrentChangeDelegate(CharController newChar);
        public static OnCurrentChangeDelegate OnCurrentChange;
        
        private void Awake()
        {
            _playOrder = new List<CharController>();
            _playOrder = GameObject.FindGameObjectsWithTag("Character").Select(obj => obj.GetComponent<CharController>()).ToList();
            //Dangerous
        }

        private void Start()
        {
            StartCoroutine(UpdateLoop());
        }

        private IEnumerator UpdateLoop()
        {
            print("SetUp");
            _playOrder.Sort((l, r) => l.Initiative.CompareTo(r.Initiative));
            DisplayOrder();
            print("SetUp Phase Ends");
            
            while(true)
            {
                yield return StartCoroutine(NextPlayer());
                StartCoroutine(PlayerPhase());
                yield return WaitTillNextPhase();
            }
        }

        private IEnumerator PlayerPhase()
        {
            print($"Now its {_current.name}s Turn");
            while (!_nextPhasePressed)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _current.Initiative++;
                    print(_current);
                }

                yield return null;
            }

            print("Player Phase End");
        }

        private IEnumerator WaitTillNextPhase()
        {
            _nextPhasePressed = false;
            nextPhaseAction.action.Enable();
            yield return new WaitUntil(() => _nextPhasePressed);
            nextPhaseAction.action.Disable();
            _nextPhasePressed = false;
        }

        private IEnumerator NextPlayer()
        {
            if (_current is not null)
            {
                _playOrder.Add(_current);
                _playOrder.Sort((l, r) => l.Initiative.CompareTo(r.Initiative));
                DisplayOrder();
            }
            
            //Reorder
            int min = _playOrder.Min(c => c.Initiative);
            if (min != 0)
            {
                yield return new WaitForSeconds(2f);
                foreach (CharController p in _playOrder)
                {
                    p.Initiative -= min;
                }
                DisplayOrder();
            }
            
            _current = _playOrder[0];
            _playOrder.RemoveAt(0);
            
            OnCurrentChange.Invoke(_current);
        }
        
        private void SetNextPhaseFlag(InputAction.CallbackContext _) => _nextPhasePressed = true;
        
        private void OnEnable()
        {
            nextPhaseAction.action.performed += SetNextPhaseFlag;
        }

        private void OnDisable()
        {
            nextPhaseAction.action.performed -= SetNextPhaseFlag;
        }

        private void DisplayOrder()
        {
            OnOrderChanged.Invoke(_playOrder.ToArray());
            //print(_playOrder.Aggregate("Current Order:\n",(current, c) => current + $"{c}\n"));
        }
    }
}