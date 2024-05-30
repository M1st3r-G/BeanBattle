using Data;
using Managers;
using UnityEngine;

namespace Controller
{
    public class CharStateController: MonoBehaviour
    {
        #region Fields

        // ComponentReferences
        [SerializeField] private StateLibrary stateLibrary;
        // Temps
        private CharacterStateBase _currentState;

        #endregion

        #region StateVariables

        //GeneralStateVariables
        public CharController MyCharacter { get; private set; }

        // MovementStateVariables
        public Vector2Int[] path;
        
        // AttackStateVariables
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
            if (CustomInputManager.Instance.StoppedThisFrame()) DisassembleCurrentState();
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
            _currentState.StateDisassembly(this);
            _currentState = stateLibrary.EmptyState;
        }

        private void OnPlayerDeath(CharController c)
        {
            if (CurrentSelection == c) CurrentSelection = null;
        }

        #endregion
    }
}