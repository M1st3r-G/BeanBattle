using Data;
using Tutorial;
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
        
        // Requests
        [SerializeField] private InputActionReference mouseClick;
        [SerializeField] private InputActionReference acceptAction;
        [SerializeField] private InputActionReference stopAction;
        // Temps
        private bool _isListeningToInput;

        // Publics
        public static CustomInputManager Instance { get; private set; }

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
            numberAction.action.Enable();
            numberAction.action.performed += NumberPressed;
            nextPhaseAction.action.Enable();
            nextPhaseAction.action.performed += EndPhase;
            
            mouseClick.action.Enable();
            acceptAction.action.Enable();
            stopAction.action.Enable();
        }

        private void OnDisable()
        {
            numberAction.action.performed -= NumberPressed;
            numberAction.action.Disable();
            nextPhaseAction.action.performed -= EndPhase;
            nextPhaseAction.action.Disable();
            
            mouseClick.action.Disable();
            acceptAction.action.Disable();
            stopAction.action.Enable();
        }

        #endregion

        #region PhaseManagingInput

        private void EndPhase(InputAction.CallbackContext _)
        {
            if (!_isListeningToInput) return;
            
            if(GameManager.Instance is null) TutorialManager.Instance.Continue();
            else GameManager.Instance.OnEndPhaseAction();
        }

        #endregion
        
        #region ActionInputManaging

        public void ActionCellPressed(CharacterAction action) => SelectAction(action);
        private void NumberPressed(InputAction.CallbackContext ctx)
        {
            // The Value [1;9] describes the number key pressed
            int index = (int)ctx.ReadValue<float>() - 1;
            CharacterAction action = UIManager.Instance.GetActionWithIndex(index);
            SelectAction(action);
        }
        
        /// <summary>
        /// Triggered either by Clicking on the Cell (<see cref="ActionCellPressed"/>) or the number buttons on the Keyboard (<see cref="NumberPressed"/>))
        /// UI is taken Care of by the State in <see cref="Controller.CharStateController"/>
        /// </summary>
        /// <param name="action">The <see cref="CharacterAction"/> action triggered</param>
        private void SelectAction(CharacterAction action)
        {
            //If Input is Enabled, it Selects Actions. The Methods handle disabling when the Same Action is Triggered again
            if (!_isListeningToInput) return;
            
            AudioEffectsManager.Instance.PlayEffect(AudioEffectsManager.AudioEffect.Click);

            if (GameManager.Instance is null) TutorialManager.Instance.Continue();
            else GameManager.Instance.TriggerState(action.Type);
        }

        public bool AcceptedThisFrame() => acceptAction.action.WasPerformedThisFrame() && _isListeningToInput;
        public bool MouseClickedThisFrame() => mouseClick.action.WasPerformedThisFrame() && _isListeningToInput;
        public bool StoppedThisFrame() => stopAction.action.WasPerformedThisFrame() && _isListeningToInput;
        #endregion

        #region EnablingAndDisablingInput

        public void EnableInput() => SetInputAction(true);
        public void DisableInput() => SetInputAction(false);
        
        private void SetInputAction(bool state) => _isListeningToInput = state;

        #endregion
    }
}