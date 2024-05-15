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
        }
        
        private void OnDisable()
        {
            GameManager.OnCurrentChange -= OnChangeEvent;
            GameManager.OnGameOver -= OnGameOver;
        }
        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
        #endregion

        #region MainMethods
        public void SetNumberActions(bool state)
        {
            if (state)
            {
                numberAction.action.Enable();
                numberAction.action.performed += NumberPressed;
            }
            else
            {
                numberAction.action.performed -= NumberPressed;
                numberAction.action.Disable();
            }
        }
        
        private void SelectAction(int actionIndex)
        {
            if (actionIndex > _current.Actions.Count)
            {
                Debug.Log($"Action{actionIndex} is not Available");
                return;
            }

            if(actionIndex -1 == _actionController.CurrentSelection) DeselectCurrentAction();
            else _actionController.Select(actionIndex - 1);
            
            GameManager.Instance.TriggerState(_current.Actions[actionIndex - 1].Type);
        }

        private void NumberPressed(InputAction.CallbackContext ctx) => SelectAction((int)ctx.ReadValue<float>());
        public void DeselectCurrentAction() => _actionController.Deselect();
        public void ActionCellPressed(int index) => SelectAction(index);
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