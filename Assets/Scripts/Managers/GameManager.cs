using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Data;
using UI.CurrentCharacter;
using UI.Initiative;
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
        private bool _gameLoop;
        
        public delegate void OnOrderChangeDelegate(CharController[] newOrder);
        public static OnOrderChangeDelegate OnOrderChanged;
        public delegate void OnCurrentChangeDelegate(CharController newChar);
        public static OnCurrentChangeDelegate OnCurrentChange;
        
        public delegate void OnGameOverDelegate(int winningTeam);
        public static OnGameOverDelegate OnGameOver;
        
        public static GameManager Instance { get; private set;  }
        
        private void Awake()
        {
            if (Instance is not null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;
            
            
            _playOrder = new List<CharController>();
            _playOrder = GameObject.FindGameObjectsWithTag("Character").Select(obj => obj.GetComponent<CharController>()).ToList();
            //Dangerous

            _gameLoop = true;
        }

        private void Start()
        {
            StartCoroutine(UpdateLoop());
        }

        private IEnumerator UpdateLoop()
        {
            _playOrder.Sort((l, r) => l.Initiative.CompareTo(r.Initiative));
            DisplayOrder();
            print("SetUp Phase Finished");
            
            while(_gameLoop)
            {
                yield return StartCoroutine(NextPlayer());
                yield return WaitTillNextPhase();
            }
        }

        private IEnumerator WaitTillNextPhase()
        {
            _nextPhasePressed = false;
            nextPhaseAction.action.Enable();
            yield return new WaitUntil(() => _nextPhasePressed);
            CurrentCharacterUIController.Instance.SetNumberActions(false);
            nextPhaseAction.action.Disable();
            _nextPhasePressed = false;
        }

        private IEnumerator NextPlayer()
        {
            if (_current is not null)
            {
                _current.EndState();
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
            
            CurrentCharacterUIController.Instance.SetNumberActions(true); // Enables number Shortcuts
        }
        
        private void SetNextPhaseFlag(InputAction.CallbackContext _) => _nextPhasePressed = true;
        
        private void OnEnable()
        {
            nextPhaseAction.action.performed += SetNextPhaseFlag;
            CharController.OnPlayerDeath += RemoveDeadPlayer;
        }

        private void RemoveDeadPlayer(CharController player)
        {
            _playOrder.Remove(player);
            int membersLeft = _playOrder.Count(c => c.TeamID == player.TeamID);
            if (membersLeft != 0) return;

            int otherTeamId = 1 - player.TeamID;
            
            Debug.Log($"Game Over, team {otherTeamId} won!");
            _gameLoop = false;
            SetNextPhaseFlag(new InputAction.CallbackContext());
            OnGameOver?.Invoke(otherTeamId);
        }

        private void OnDisable()
        {
            nextPhaseAction.action.performed -= SetNextPhaseFlag;
            CharController.OnPlayerDeath -= RemoveDeadPlayer;
        }

        public void RefreshInitiative(CharController c)
        {
            InitiativeUIController.Instance.RefreshCharacter(c);
            
            if (c.Initiative < 10) return;
            SetNextPhaseFlag(new InputAction.CallbackContext());
        }
        
        public void TriggerState(CharacterAction.ActionTypes type)
        {
            _current.TriggerState(type);
        }
        
        private void DisplayOrder()
        {
            OnOrderChanged.Invoke(_playOrder.ToArray());
        }
    }
}