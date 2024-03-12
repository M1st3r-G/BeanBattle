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
            print("SetUp");
            StartCoroutine(SetUpPhase());
            yield return WaitTillNextPhase();
            
            while(true)
            {
                NextPlayer();
                print("PlayerPhase");
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
                    print(_current);
                }

                yield return null;
            }

            print("Phase End");
        }
        
        private IEnumerator SetUpPhase()
        {
            while (!_nextPhasePressed)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    print("Add Player");
                    GameObject newPlayer = Instantiate(player);
                    _playOrder.Add(newPlayer.GetComponent<CharController>());
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    print("Add Enemy");
                    GameObject newEnemy = Instantiate(enemy);
                    _playOrder.Add(newEnemy.GetComponent<CharController>());
                }

                yield return null;
            }
            
            _playOrder.Sort((l, r) => l.Initiative.CompareTo(r.Initiative));
            print(DisplayOrder());
        }
        
        private IEnumerator WaitTillNextPhase()
        {
            _nextPhasePressed = false;
            nextPhaseAction.action.Enable();
            yield return new WaitUntil(() => _nextPhasePressed);
            nextPhaseAction.action.Disable();
            _nextPhasePressed = false;
        }

        private void NextPlayer()
        {
            if (_current is not null)
            {
                _playOrder.Add(_current);
                _playOrder.Sort((l, r) => l.Initiative.CompareTo(r.Initiative));
                print(DisplayOrder());
            }
            
            //Reorder
            int min = _playOrder.Min(c => c.Initiative);
            if (min != 0)
            {
                foreach (CharController p in _playOrder)
                {
                    p.Initiative -= min;
                }
                print(DisplayOrder());
            }
            
            _current = _playOrder[0];
            _playOrder.RemoveAt(0);
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

        private string DisplayOrder() => _playOrder.Aggregate("Current Order:\n", 
            (current, c) => current + $"{c}\n");
    }
}