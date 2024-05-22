using Controller;
using Data;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.CurrentCharacter
{
    public class CurrentCharacterUIController : MonoBehaviour
    {
        //ComponentReferences
        private ActionsUIController _actionController;
        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI nameText; 
        [SerializeField] private InputActionReference numberAction;
        
        //Temps
        private CharData _current;
        private bool _isAnimating;

        //Public
        public static CurrentCharacterUIController Instance { get; private set; }

        #region SetUp
        private void Awake()
        {
            if (Instance is not null){
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _actionController = GetComponentInChildren<ActionsUIController>();
        }
        
        private void OnEnable()
        {
            GameManager.OnCurrentChange += OnChangeEvent;
            GameManager.OnGameOver += OnGameOver;
            CharController.OnPlayerStartedAnimation += OnPlayerStartedAction;
            CharController.OnPlayerFinishedAnimation += OnPlayerFinishedAction;
            
            numberAction.action.Enable();
            numberAction.action.performed += NumberPressed;
        }
        
        private void OnDisable()
        {
            GameManager.OnCurrentChange -= OnChangeEvent;
            GameManager.OnGameOver -= OnGameOver;
            CharController.OnPlayerStartedAnimation -= OnPlayerStartedAction;
            CharController.OnPlayerFinishedAnimation -= OnPlayerFinishedAction;
            
            numberAction.action.performed -= NumberPressed;
            numberAction.action.Disable();
        }

        private void OnPlayerFinishedAction(CharacterAction.ActionTypes type) => SetActionInput(true);
        private void OnPlayerStartedAction(CharacterAction.ActionTypes type) => SetActionInput(false);

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
        #endregion

        #region MainMethods
        public void SetActionInput(bool state) => _isAnimating = !state;
        
        private void NumberPressed(InputAction.CallbackContext ctx) => SelectAction((int)ctx.ReadValue<float>());
        public void ActionCellPressed(int index) => SelectAction(index);
        
        private void SelectAction(int actionIndex)
        {
            if (actionIndex > _current.Actions.Count)
            {
                Debug.Log($"Action{actionIndex} is not Available");
                return;
            }

            if (_isAnimating) return;

            if (actionIndex - 1 == _actionController.CurrentSelection)
            {
                DeselectCurrentAction();
                GameManager.Instance.TriggerState(CharacterAction.ActionTypes.None);
            }
            else
            {
                _actionController.Select(actionIndex - 1);
                GameManager.Instance.TriggerState(_current.Actions[actionIndex - 1].Type);
            }
            
        }

        private void DeselectCurrentAction() => _actionController.Deselect();
        #endregion
        
        #region EventHandling
        private void OnGameOver(int winningTeam)
        {
            DeselectCurrentAction();
            gameObject.SetActive(false);
        }
        
        private void OnChangeEvent(CharController newChar)
        {
            _current = newChar.GetData;
            _actionController.SetDisplay(_current.Actions);
            portrait.color = _current.Material(newChar.TeamID).color;
            nameText.text = _current.Name;
        }
        #endregion
    }
}