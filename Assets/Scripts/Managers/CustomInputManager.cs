using Controller;
using Data;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class CustomInputManager : MonoBehaviour
    {
        [SerializeField] private InputActionReference numberAction;
        private bool _isListeningToInput;

        public static CustomInputManager Instance { get; private set; }


        private void Awake()
        {
            if (Instance is not null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            CharController.OnPlayerStartedAction += OnPlayerStartedAction;
            CharController.OnPlayerFinishedAction += OnPlayerFinishedAction;
            
            numberAction.action.Enable();
            numberAction.action.performed += NumberPressed;
        }

        private void OnDisable()
        {
            CharController.OnPlayerStartedAction -= OnPlayerStartedAction;
            CharController.OnPlayerFinishedAction -= OnPlayerFinishedAction;
            
            numberAction.action.performed -= NumberPressed;
            numberAction.action.Disable();
        }
        
        private void NumberPressed(InputAction.CallbackContext ctx)
        {
            int index = (int)ctx.ReadValue<float>() - 1;
            CharacterAction action = UIManager.Instance.GetActionWithIndex(index);
            SelectAction(index, action);
        }

        public void ActionCellPressed(int index, CharacterAction action) => SelectAction(index, action);
        
        private void OnPlayerFinishedAction(CharacterAction.ActionTypes type) => EnableInputAction(true);
        private void OnPlayerStartedAction(CharacterAction.ActionTypes type) => EnableInputAction(false);
        
        public void EnableInputAction(bool state) => _isListeningToInput = state;
        
        private void SelectAction(int actionIndex, CharacterAction action)
        {
            
            if (!_isListeningToInput) return;
            UIManager.Instance.SelectAction(actionIndex);
            GameManager.Instance.TriggerState(action.Type);
        }
    }
}