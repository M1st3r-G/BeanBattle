using Data;
using UI.CurrentCharacter;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class CharStateController: MonoBehaviour
    {
        // ComponentReferences
        [SerializeField] private InputActionReference stopAction;
        [SerializeField] private StateLibrary stateLibrary;
        
        // Temps
        private CharacterStateBase _currentState;

        //State Variables
        
        //General
        public CharController MyCharacter { get; private set; }
        public InputAction MouseClickAction => mouseClick.action;
        [SerializeField] private InputActionReference mouseClick;
        public bool IsAnimating { get; set; }

        //Movement
        public float TimePerSpace => timePerSpace;
        [SerializeField] private float timePerSpace;
        
        // Attacks
        public InputActionReference AcceptAction => acceptAction;
        [SerializeField] private InputActionReference acceptAction;
        
        public CharController CurrentSelection { get;  set; }
        public bool LookingForPlayer { get;  set; }
        
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
                DisassembleCurrentState();
                MyCharacter.AddInitiative();
            }
            if (stopAction.action.WasPerformedThisFrame()) DisassembleCurrentState();
        }
        #endregion

        #region StateManagement
        public void SwitchState(CharacterAction.ActionTypes targetState)
        {
            DisassembleCurrentState();
            if (_currentState.ActionType == targetState) return;

            stopAction.action.Enable();
            _currentState = targetState switch
            {
                CharacterAction.ActionTypes.None => stateLibrary.EmptyState,
                _ => stateLibrary.GetState(targetState)
            };

            _currentState.StateSetUp(this);
        }
        
        private void DisassembleCurrentState()
        {
            stopAction.action.Disable();
            _currentState.StateDisassembly(this);
            _currentState = stateLibrary.EmptyState;
        }

        private void PlayerDeath(CharController c) => _currentState.OnPlayerDeath(this, c);
        #endregion
    }
}