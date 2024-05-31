using Data;
using Data.CharacterStates;
using Managers;
using UnityEngine;

namespace Controller
{
    public class CharStateController: MonoBehaviour
    {
        #region Fields
        
        // Component References
        [SerializeField] private StateLibrary stateLibrary;
        // Temps
        private CharacterStateBase _currentState;

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

        private void OnPlayerDeath(CharController c)
        {
            if (CurrentSelection == c) CurrentSelection = null;
        }
        
        #endregion
        
        #region StateManagement
        
        public void TriggerState(CharacterAction.ActionTypes type)
        {
            Debug.Assert(type != CharacterAction.ActionTypes.None, "Der None State sollte getriggert werden. Das ist nicht vorgesehen");
            
            if (_currentState.ActionType == CharacterAction.ActionTypes.None)
            {
                SetNewState(type);
            }
            else
            {
                if (_currentState.ActionType == type)
                {
                    RemoveState();
                }
                else //other State
                {
                    RemoveState();
                    SetNewState(type);
                }
            }
        }

        private void RemoveState()
        {
            _currentState.StateDisassembly(this);
            _currentState = stateLibrary.EmptyState;
            UIManager.Instance.DeselectCurrentAction();
        }
        
        private void SetNewState(CharacterAction.ActionTypes type)
        {
            _currentState = stateLibrary.GetState(type);
            _currentState.StateSetUp(this);
            UIManager.Instance.DisplayNewAction(type);
        }
        
        private void Update()
        {
            if (_currentState.ExecuteStateFrame(this) || CustomInputManager.Instance.StoppedThisFrame()) RemoveState();
        }
        
        #endregion
    }
}