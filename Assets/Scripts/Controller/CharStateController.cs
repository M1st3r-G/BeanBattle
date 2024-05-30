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

        // AttackStateVariables
        public InputActionReference AcceptAction => acceptAction;
        [SerializeField] private InputActionReference acceptAction;
        public CharController CurrentSelection { get;  set; }
        public bool LookingForSelection { get;  set; }

        #endregion
        
        #region SetUp
        
        private void Awake()
        {
            MyCharacter = GetComponent<CharController>();
            _currentState = stateLibrary.EmptyState;
        }

        private void OnEnable()
        {
            CharController.OnPlayerDeath += OnPlayerDeath;
        }

        private void OnDisable()
        {
            CharController.OnPlayerDeath -= OnPlayerDeath;
        }

        #endregion

        #region MainLoop
        
        private void Update()
        {
            if (_currentState.ExecuteStateFrame(this))
            {
                //On Legal Exit
                MyCharacter.AddInitiative(UIManager.Instance.GetTimeCost());
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
            CharacterAction.ActionTypes previousState = _currentState.ActionType;
            DisassembleCurrentState();
            if (previousState == targetState) return;

            // Set up new State
            stopAction.action.Enable();
            
            _currentState = targetState switch
            {
                CharacterAction.ActionTypes.None => stateLibrary.EmptyState,
                _ => stateLibrary.GetState(targetState)
            };
            
            Debug.Log($"Setting Up the {_currentState} State");
            _currentState.StateSetUp(this);
        }
        
        /// <summary>
        /// Changes to The EmptyState safely
        /// </summary>
        private void DisassembleCurrentState()
        {
            Debug.Log($"Disabling the {_currentState.ActionType} State");
            stopAction.action.Disable();
            _currentState.StateDisassembly(this);
            
            if(!IsAnimating) CharController.OnPlayerFinishedAction?.Invoke(_currentState.ActionType);
            _currentState = stateLibrary.EmptyState;
        }

        /// <summary>
        /// Changes State Variables that could Lead to Errors
        /// </summary>
        /// <param name="c">The Dead Player</param>
        private void OnPlayerDeath(CharController c)
        {
            if (CurrentSelection == c) CurrentSelection = null;
        }

        #endregion
    }
}