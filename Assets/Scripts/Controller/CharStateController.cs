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
        private CharController myCharacter;
        
        // Temps
        private CharacterStateBase currentState;

        #region SetUp
        private void Awake()
        {
            myCharacter = GetComponent<CharController>();
            currentState = stateLibrary.EmptyState;
        }
        #endregion

        #region MainLoop
        private void Update()
        {
            if (currentState.ExecuteStateFrame())
            {
                EndCurrentState();
                //Animate
                myCharacter.AddInitiative();
            }
            if (stopAction.action.WasPerformedThisFrame()) EndCurrentState();
        }
        #endregion

        #region StateManagement
        public void SwitchState(CharacterAction.ActionTypes targetState)
        {
            if (currentState.Type == targetState)
            {
                EndCurrentState();
                return;
            }
            
            DisassembleCurrentState();

            stopAction.action.Enable();
            currentState = targetState switch
            {
                CharacterAction.ActionTypes.None => stateLibrary.EmptyState,
                _ => stateLibrary.GetState(targetState)
            };

            currentState.StateSetUp(myCharacter);
        }
        
        private void DisassembleCurrentState()
        {
            currentState.StateDisassembly();
            currentState = stateLibrary.EmptyState;
            stopAction.action.Disable();
        }
        
        private void EndCurrentState()
        {
            DisassembleCurrentState();
            CurrentCharacterUIController.Instance.DeselectCurrentAction();
        }
        #endregion
    }
}