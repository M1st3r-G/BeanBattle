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

        [SerializeField] private InputActionReference numberAction;
        [SerializeField] private InputActionReference nextPhaseAction;
        private bool _isListeningToInput;

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
            CharController.OnPlayerStartedAction += OnPlayerStartedAction;
            CharController.OnPlayerFinishedAction += OnPlayerFinishedAction;
            
            numberAction.action.Enable();
            numberAction.action.performed += NumberPressed;
            nextPhaseAction.action.Enable();
            nextPhaseAction.action.performed += EndPhase;
        }

        private void OnDisable()
        {
            CharController.OnPlayerStartedAction -= OnPlayerStartedAction;
            CharController.OnPlayerFinishedAction -= OnPlayerFinishedAction;
            
            numberAction.action.performed -= NumberPressed;
            numberAction.action.Disable();
            nextPhaseAction.action.performed -= EndPhase;
            nextPhaseAction.action.Disable();
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

        #endregion

        #region EnablingAndDisablingInput

        private void OnPlayerFinishedAction(CharacterAction.ActionTypes type) => EnableInputAction(true);
        private void OnPlayerStartedAction(CharacterAction.ActionTypes type) => EnableInputAction(false);
        
        public void EnableInputAction(bool state) => _isListeningToInput = state;

        #endregion
    }
}