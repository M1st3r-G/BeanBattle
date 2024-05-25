using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Data;
using UI;
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
        private CharController CurrentPlayer { get; set; }
        private List<CharController> _playOrder;
        private bool _nextPhasePressed;
        private bool _gameLoop;
        
        //Public
        public static GameManager Instance { get; private set;  }
        
        //Events
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
            UIManager.Instance.ChangeInitiativeOrderTo(_playOrder.ToArray());
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
            nextPhaseAction.action.performed += EndPhase;
            CharController.OnPlayerDeath += RemoveDeadPlayer;
        }
        
        private void OnDisable()
        {
            nextPhaseAction.action.performed -= EndPhase;
            CharController.OnPlayerDeath -= RemoveDeadPlayer;
        }
        
        #endregion

        #region EventMethods
        
        private void EndPhase(InputAction.CallbackContext _) => TriggerNextRound();
        
        private void RemoveDeadPlayer(CharController player)
        {
            _playOrder.Remove(player);
            
            if (_playOrder.Count(c => c.TeamID == player.TeamID) == 0) 
                TriggerGameOver(1 - player.TeamID); // Team is Dead
        }

        #endregion
        
        #region MainLoop

        private IEnumerator UpdateLoop()
        {
            while(_gameLoop)
            {
                CustomInputManager.Instance.EnableInputAction(false);
                nextPhaseAction.action.Disable();
                
                yield return NextPlayer();
                
                UIManager.Instance.ChangeActiveCharacter(CurrentPlayer);
                
                CustomInputManager.Instance.EnableInputAction(true); // Enables number Shortcuts
                nextPhaseAction.action.Enable();
                _nextPhasePressed = false;
                
                yield return new WaitUntil(() => _nextPhasePressed);
            }
        }

        private IEnumerator NextPlayer()
        {
            if (CurrentPlayer is not null)
            {
                CurrentPlayer.EndState();
                _playOrder.Add(CurrentPlayer);
                _playOrder.Sort((l, r) => l.Initiative.CompareTo(r.Initiative));
                UIManager.Instance.ChangeInitiativeOrderTo(_playOrder.ToArray());
            }
            
            //Count Time Down
            int min = _playOrder.Min(c => c.Initiative);
            if (min > 0)
            {
                foreach (CharController p in _playOrder) p.Initiative--;
                int counter = 1;
                while (counter >= min)
                {
                    yield return new WaitForSeconds(0.5f);
                    foreach (CharController p in _playOrder) p.Initiative--;
                    counter++;
                }
                
                UIManager.Instance.ChangeInitiativeOrderTo(_playOrder.ToArray());
            }
            
            CurrentPlayer = _playOrder[0];
            _playOrder.RemoveAt(0);
        }
        
        #endregion

        #region OtherMethods

        public void TriggerNextRound() => _nextPhasePressed = true;
        
        public void TriggerState(CharacterAction.ActionTypes type) => CurrentPlayer.TriggerState(type);

        private void TriggerGameOver(int winningTeam)
        {
            _gameLoop = false;
            TriggerNextRound();
            OnGameOver?.Invoke(winningTeam);
        }
        
        #endregion
    }
}