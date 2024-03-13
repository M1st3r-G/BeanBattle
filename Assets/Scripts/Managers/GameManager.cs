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
        [SerializeField] private GameObject enemy;
        [SerializeField] private GameObject player;
        
        
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
        }

        private void Start()
        {
            StartCoroutine(UpdateLoop());
        }

        private IEnumerator UpdateLoop()
        {
            StartCoroutine(SetUpPhase());
            yield return WaitTillNextPhase();
            
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
        
        private IEnumerator SetUpPhase()
        {
            print("SetUp");
            while (!_nextPhasePressed)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    GameObject newPlayer = Instantiate(player, Vector3.left * 7 + Vector3.right * _playOrder.Count, Quaternion.identity);
                    _playOrder.Add(newPlayer.GetComponent<CharController>());
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    GameObject newEnemy = Instantiate(enemy, Vector3.left * 7 + Vector3.right * _playOrder.Count, Quaternion.identity);
                    _playOrder.Add(newEnemy.GetComponent<CharController>());
                }

                if (_playOrder.Count >= 12) break;
                yield return null;
            }
            
            _playOrder.Sort((l, r) => l.Initiative.CompareTo(r.Initiative));
            DisplayOrder();
            print("SetUp Phase Ends");
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