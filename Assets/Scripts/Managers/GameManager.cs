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
        //ComponentReferences
        [SerializeField] private InputActionReference nextPhaseAction;
        [SerializeField] private CharData[] characterClasses;
        [SerializeField] private GameObject characterPrefab;
        
        //Temps
        private List<CharController> _playOrder;
        private CharController _current;
        private bool _nextPhasePressed;
        private bool _gameLoop;
        
        //Publics
        public static GameManager Instance { get; private set;  }
        
        //Events
        public delegate void OnOrderChangeDelegate(CharController[] newOrder);
        public static OnOrderChangeDelegate OnOrderChanged;
        public delegate void OnCurrentChangeDelegate(CharController newChar);
        public static OnCurrentChangeDelegate OnCurrentChange;
        public delegate void OnGameOverDelegate(int winningTeam);
        public static OnGameOverDelegate OnGameOver;

        #region SetUp

        private void Awake()
        {
            if (Instance is not null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;

            _gameLoop = true;
        }
        
        private void Start()
        {
            SetUp();
            StartCoroutine(UpdateLoop());
        }
        
        private void SetUp()
        {
            _playOrder = GenerateCharacters().ToList();
            
            _playOrder.Sort((l, r) => l.Initiative.CompareTo(r.Initiative));
            OnOrderChanged.Invoke(_playOrder.ToArray());
            print("SetUp Phase Finished");
        }
        
        private IEnumerable<CharController> GenerateCharacters()
        {
            List<CharController> returnCollection = new List<CharController>();
            
            for (int team = 0; team < 2; team++)
            {
                for (int i = 0; i < characterClasses.Length; i++)
                {
                    CharController tmp = Instantiate(characterPrefab).GetComponent<CharController>();
                    tmp.Init(characterClasses[i], team);

                    GridManager.Instance.AddCharacter(tmp, new Vector2Int(12 * team + 1, 3 * i + 4));
                    returnCollection.Add(tmp);
                }
            }

            return returnCollection;
        }

        private void OnEnable()
        {
            nextPhaseAction.action.performed += SetNextPhaseFlag;
            CharController.OnPlayerDeath += RemoveDeadPlayer;
        }
        
        private void OnDisable()
        {
            nextPhaseAction.action.performed -= SetNextPhaseFlag;
            CharController.OnPlayerDeath -= RemoveDeadPlayer;
        }
        #endregion

        #region MainLoop

        private IEnumerator UpdateLoop()
        {
            while(_gameLoop)
            {
                yield return NextPlayer();
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
        
        private void SetNextPhaseFlag(InputAction.CallbackContext _) => _nextPhasePressed = true;

        private IEnumerator NextPlayer()
        {
            if (_current is not null)
            {
                _current.EndState();
                _playOrder.Add(_current);
                _playOrder.Sort((l, r) => l.Initiative.CompareTo(r.Initiative));
                OnOrderChanged.Invoke(_playOrder.ToArray());
            }
            
            //Count Time Down
            int min = _playOrder.Min(c => c.Initiative);
            if (min > 0)
            {
                yield return new WaitForSeconds(2f);
                
                foreach (CharController p in _playOrder) p.Initiative -= min;
                OnOrderChanged.Invoke(_playOrder.ToArray());
            }
            
            _current = _playOrder[0];
            _playOrder.RemoveAt(0);
            
            OnCurrentChange.Invoke(_current);
            CurrentCharacterUIController.Instance.SetNumberActions(true); // Enables number Shortcuts
        }
        #endregion

        #region OtherMethods

        public void TriggerState(CharacterAction.ActionTypes type) => _current.TriggerState(type);
        
        public void RefreshInitiative(CharController c)
        {
            InitiativeUIController.Instance.RefreshCharacter(c);
            
            if (c.Initiative < 10) return;
            SetNextPhaseFlag(new InputAction.CallbackContext());
        }
        
        private void RemoveDeadPlayer(CharController player)
        {
            _playOrder.Remove(player);
            
            if (_playOrder.Count(c => c.TeamID == player.TeamID) != 0) return;
            // Team is Dead
            _gameLoop = false;
            SetNextPhaseFlag(new InputAction.CallbackContext());
            OnGameOver?.Invoke(1 - player.TeamID);
        }
        #endregion
    }
}