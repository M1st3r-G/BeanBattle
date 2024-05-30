using Controller;
using Data;
using UIContent.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class CustomInputManager : MonoBehaviour
    {
        #region Fields
        // Component References
        [SerializeField] private InputActionReference numberAction;
        [SerializeField] private InputActionReference nextPhaseAction;
        [SerializeField] private InputActionReference mouseClick;
        [SerializeField] private InputActionReference acceptAction;
        [SerializeField] private InputActionReference stopAction;
        // Temps
        private bool _isListeningToInput;

        // Publics
        public static CustomInputManager Instance { get; private set; }

        public delegate void EnableInputDelegate(CharacterAction.ActionTypes type);
        public static EnableInputDelegate EnableInputEvent;
        public delegate void DisableInputDelegate(CharacterAction.ActionTypes type);
        public static DisableInputDelegate DisableInputEvent;
        
        #endregion

        #region SetUp

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void OnEnable()
        {
            DisableInputEvent += DisableInputAction;
            EnableInputEvent += EnableInputAction;
            
            numberAction.action.Enable();
            numberAction.action.performed += NumberPressed;
            nextPhaseAction.action.Enable();
            nextPhaseAction.action.performed += EndPhase;
            
            mouseClick.action.Enable();
            acceptAction.action.Enable();
        }

        private void OnDisable()
        {
            DisableInputEvent -= DisableInputAction;
            EnableInputEvent -= EnableInputAction;
            
            numberAction.action.performed -= NumberPressed;
            numberAction.action.Disable();
            nextPhaseAction.action.performed -= EndPhase;
            nextPhaseAction.action.Disable();
            
            mouseClick.action.Disable();
            acceptAction.action.Disable();
        }

        #endregion

        #region PhaseManagingInput

        private void EndPhase(InputAction.CallbackContext _)
        {
            if (_isListeningToInput) GameManager.Instance.EndPhase();
        }

        #endregion
        
        #region ActionInputManaging

        private void NumberPressed(InputAction.CallbackContext ctx)
        {
            // The Value [1;9] describes the number key pressed
            int index = (int)ctx.ReadValue<float>() - 1;
            CharacterAction action = UIManager.Instance.GetActionWithIndex(index);
            SelectAction(index, action);
        }

        public void ActionCellPressed(int index, CharacterAction action) => SelectAction(index, action);
        
        /// <summary>
        /// Triggered either by Clicking on the Cell (<see cref="ActionCellPressed"/>) or the number buttons on the Keyboard (<see cref="NumberPressed"/>))
        /// </summary>
        /// <param name="actionIndex">The Index (Zero Based) of the Action in the <see cref="ActionsUI"/> list</param>
        /// <param name="action">The <see cref="CharacterAction"/> action triggered</param>
        private void SelectAction(int actionIndex, CharacterAction action)
        {
            //If Input is Enabled, it Selects Actions. The Methods handle disabling when the Same Action is Triggered again
            if (!_isListeningToInput) return;
            
            AudioEffectsManager.Instance.PlayEffect(AudioEffectsManager.AudioEffect.Click);
            UIManager.Instance.SelectAction(actionIndex);
            GameManager.Instance.TriggerState(action.Type);
        }

        public bool AcceptedThisFrame() => acceptAction.action.WasPerformedThisFrame() && _isListeningToInput;
        public bool MouseClickedThisFrame() => mouseClick.action.WasPerformedThisFrame() && _isListeningToInput;
        public bool StoppedThisFrame() => stopAction.action.WasPerformedThisFrame() && _isListeningToInput;
        #endregion

        #region EnablingAndDisablingInput

        private void EnableInputAction(CharacterAction.ActionTypes type) => SetInputAction(true);
        private void DisableInputAction(CharacterAction.ActionTypes type) => SetInputAction(false);
        
        private void SetInputAction(bool state) => _isListeningToInput = state;

        #endregion
    }
}