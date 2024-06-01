using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Data;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        #region Fields

        //ComponentReferences
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

        //Data
        private int totalSteps;
        private int totalTurns;
        
        #endregion

        #region SetUp

        private void Awake()
        {
            Instance = this;

            _gameLoop = true;
            totalSteps = totalTurns = 0;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Start()
        {
            SetUp();
            StartCoroutine(UpdateLoop());
        }
        
        private void SetUp()
        {
            _playOrder = GenerateCharacters();
            
            _playOrder.Sort((l, r) => l.Initiative.CompareTo(r.Initiative));
            UIManager.Instance.ChangeInitiativeOrderTo(_playOrder.ToArray());
        }
        
        /// <summary>
        /// Generates a Character for each team and of each Type
        /// </summary>
        /// <returns>A List of the <see cref="CharController"/> belonging to the Characters</returns>
        private List<CharController> GenerateCharacters()
        {
            List<CharController> returnCollection = new List<CharController>();
            
            for (int team = 0; team < 2; team++)
            {
                for (int i = 0; i < characterClasses.Length; i++)
                {
                    CharController tmp = Instantiate(characterPrefab).GetComponent<CharController>();
                    tmp.Init(characterClasses[i], team);
                    
                    // Notify the Grid to set the StartCell Occupied
                    GridManager.Instance.AddCharacter(tmp, new Vector2Int(12 * team + 1, 3 * i + 4));
                    returnCollection.Add(tmp);
                }
            }

            return returnCollection;
        }

        private void OnEnable()
        {
            CharController.OnPlayerDeath += RemoveDeadPlayer;
        }
        
        private void OnDisable()
        {
            CharController.OnPlayerDeath -= RemoveDeadPlayer;
        }
        
        #endregion

        #region EventMethods
        
        public void OnEndPhaseAction()
        {
            if(CurrentPlayer.Initiative > 0) TriggerNextRound();
        }

        private void RemoveDeadPlayer(CharController player)
        {
            _playOrder.Remove(player);
            
            if (_playOrder.Count(c => c.TeamID == player.TeamID) == 0) 
                TriggerGameOver(1 - player.TeamID); // Team is Dead
        }

        #endregion
        
        #region MainLoop

        /// <summary>
        /// This Coroutine Replaces the UpdateLoop
        /// </summary>
        /// <returns>Irrelevant, as this is a Coroutine</returns>
        private IEnumerator UpdateLoop()
        {
            while(_gameLoop)
            {
                //Select and Display the next Player
                yield return NextPlayer();
                Debug.Log("Selected new Player");
                UIManager.Instance.ChangeActiveCharacter(CurrentPlayer);
                
                Debug.LogWarning("PlayerWindow opened");
                CustomInputManager.Instance.EnableInput(); // Enables number Shortcuts

                // Wait for EndOfPhase
                _nextPhasePressed = false;
                yield return new WaitUntil(() => _nextPhasePressed);
                CustomInputManager.Instance.DisableInput();
            }
        }

        /// <summary>
        /// This Coroutine Updates the <see cref="_playOrder"/> and Sets <see cref="CurrentPlayer"/> to the next Player.
        /// Note that the Player is Removed from the playOrder
        /// </summary>
        /// <returns>Irrelevant, as this is a Coroutine</returns>
        private IEnumerator NextPlayer()
        {
            if (CurrentPlayer is not null)
            {
                CurrentPlayer.SkipState();
                //Add him to the Order and Reorders, then Displays the List
                _playOrder.Add(CurrentPlayer);
                _playOrder.Sort((l, r) => l.Initiative.CompareTo(r.Initiative));
                UIManager.Instance.ChangeInitiativeOrderTo(_playOrder.ToArray());
            }
            
            //Count Time Down (step by Step)
            int min = _playOrder.Min(c => c.Initiative);
            if (min > 0)
            {
                int counter = 0;
                while (counter < min)
                {
                    float length = AudioEffectsManager.Instance.PlayEffect(AudioEffectsManager.AudioEffect.Ticking);
                    yield return new WaitForSeconds(length);
                    foreach (CharController p in _playOrder) p.Initiative--;
                    UIManager.Instance.ChangeInitiativeOrderTo(_playOrder.ToArray());
                    counter++;
                }
                
            }
            
            // Set and Remove the Next Player
            CurrentPlayer = _playOrder[0];
            _playOrder.RemoveAt(0);

            totalTurns++;
        }
        
        #endregion

        #region OtherMethods

        /// <summary>
        /// Triggers the Next Round
        /// </summary>
        private void TriggerNextRound()
        {
            Debug.LogWarning("Next Round Was Triggered");
            _nextPhasePressed = true;
        }

        /// <summary>
        /// Trigger a given State in the <see cref="CurrentPlayer"/> Player
        /// </summary>
        /// <param name="type">The Type of the State to Trigger</param>
        public void TriggerState(CharacterAction.ActionTypes type)
        {
            Debug.Log($"GameManager wants to Trigger the {type} State");
            CurrentPlayer.TriggerCharacterState(type);
        }

        private void TriggerGameOver(int winningTeam)
        {
            Debug.LogWarning("GameOver");
            AudioEffectsManager.Instance.PlayEffect(AudioEffectsManager.AudioEffect.Victory);
            _gameLoop = false;
            TriggerNextRound();
            OnGameOver?.Invoke(winningTeam);
        }

        public void FullActionEnd()
        {
            UIManager.Instance.DeselectCurrentAction();
            CurrentPlayer.AddInitiative(UIManager.Instance.GetTimeCost());
            if(CurrentPlayer.Initiative >= 10) TriggerNextRound();
            else CustomInputManager.Instance.EnableInput();
        }
        
        /// <summary>
        /// Returns the Number of Turns, and the Number of Steps
        /// </summary>
        /// <returns></returns>
        public Tuple<int, int> GetGameStats()
        {
            return new Tuple<int, int>(totalTurns, totalSteps);
        }
        
        public void CountSteps(int amount)
        {
            totalSteps += amount;
        }
        
        #endregion
    }
}