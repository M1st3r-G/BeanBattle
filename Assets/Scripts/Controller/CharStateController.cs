using System;
using Data;
using UI.CurrentCharacter;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class CharStateController: MonoBehaviour
    {
        private CharController myCharacter;
        [SerializeField] private InputActionReference stopAction;
        [SerializeField] private StateLibrary stateLibrary;
        
        private CharacterStateBase currentState;
        
        private void Awake()
        {
            myCharacter = GetComponent<CharController>();
            currentState = stateLibrary.EmptyState;
        }

        public void SwitchState(CharacterAction.ActionTypes targetState)
        {
            if (currentState.Type == targetState)
            {
                EndCurrentState();
                return;
            }
            
            EndCurrentState();

            stopAction.action.Enable();
            currentState = targetState switch
            {
                CharacterAction.ActionTypes.None => throw new ArgumentOutOfRangeException(nameof(targetState),
                    targetState, null),
                _ => stateLibrary.GetState(targetState)
            };

            currentState.StateSetUp(myCharacter);
        }

        private void Update()
        {
            if (currentState.ExecuteStateFrame())
            {
                EndCurrentState();
                myCharacter.AddInitiative();
            }
            if (stopAction.action.WasPerformedThisFrame()) EndCurrentState();
        }
        
        private void EndCurrentState()
        {
            currentState.StateDisassembly();
            currentState = stateLibrary.EmptyState;

            stopAction.action.Disable();
            CurrentCharacterUIController.Instance.DeselectCurrentAction();
        }
    }
}