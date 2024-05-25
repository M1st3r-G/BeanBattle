using Data;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class CharStateController: MonoBehaviour
    {
        #region Fields

        // ComponentReferences
        [SerializeField] private InputActionReference stopAction;
        [SerializeField] private StateLibrary stateLibrary;
        // Temps
        private CharacterStateBase _currentState;

        #endregion

        #region StateVariables

        //GeneralStateVariables
        public CharController MyCharacter { get; private set; }
        public InputAction MouseClickAction => mouseClick.action;
        [SerializeField] private InputActionReference mouseClick;
        public bool IsAnimating { get; set; }

        //MovementStateVariables
        public float TimePerSpace => timePerSpace;
        [SerializeField] private float timePerSpace;
        
        // AttackStateVariables
        public InputActionReference AcceptAction => acceptAction;
        [SerializeField] private InputActionReference acceptAction;
        public CharController CurrentSelection { get;  set; }
        public bool LookingForPlayer { get;  set; }

        #endregion
        
        #region SetUp
        private void Awake()
        {
            MyCharacter = GetComponent<CharController>();
            _currentState = stateLibrary.EmptyState;
        }

        private void OnEnable()
        {
            CharController.OnPlayerDeath += PlayerDeath;
        }

        private void OnDisable()
        {
            CharController.OnPlayerDeath -= PlayerDeath;
        }

        #endregion

        #region MainLoop
        
        private void Update()
        {
            if (_currentState.ExecuteStateFrame(this))
            {
                //On Legal Exit
                DisassembleCurrentState();
            }
            if (stopAction.action.WasPerformedThisFrame()) DisassembleCurrentState();
        }
        
        #endregion

        #region StateManagement
        
        /// <summary>
        /// Sets up a new State (Or cancels the current if given the type of the current state
        /// </summary>
        /// <param name="targetState">The type of the new State</param>
        public void SwitchState(CharacterAction.ActionTypes targetState)
        {
            DisassembleCurrentState();
            if (_currentState.ActionType == targetState) return;

            // Set up new State
            stopAction.action.Enable();
            
            _currentState = targetState switch
            {
                CharacterAction.ActionTypes.None => stateLibrary.EmptyState,
                _ => stateLibrary.GetState(targetState)
            };

            _currentState.StateSetUp(this);
        }
        
        /// <summary>
        /// Changes to The EmptyState safely
        /// </summary>
        private void DisassembleCurrentState()
        {
            stopAction.action.Disable();
            _currentState.StateDisassembly(this);
            if(!IsAnimating) CharController.OnPlayerFinishedAction?.Invoke(_currentState.ActionType);
            _currentState = stateLibrary.EmptyState;
        }

        /// <summary>
        /// Changes State Variables that could Lead to Errors
        /// </summary>
        /// <param name="c">The Dead Player</param>
        private void PlayerDeath(CharController c)
        {
            if (CurrentSelection == c) CurrentSelection = null;
        }

        /// <summary>
        /// Triggered When a State Fully has Ended
        /// </summary>
        /// <param name="type"></param>
        public void EndOfStateReached(CharacterAction.ActionTypes type)
        {
            CharController.OnPlayerFinishedAction?.Invoke(type);
            MyCharacter.AddInitiative(UIManager.Instance.GetTimeCost());
        }
        
        #endregion
    }
}