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

        private void Awake()
        {
            if (Instance is not null){
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _actionController = GetComponentInChildren<ActionsUIController>();
        }

        private void OnChangeEvent(CharController newChar)
        {
            _current = newChar.GetData;
            _actionController.SetDisplay(_current.Actions);
            portrait.color = _current.Material(newChar.TeamID).color;
            nameText.text = _current.Name;
        }

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
        
        private void NumberPressed(InputAction.CallbackContext ctx) => SelectAction((int)ctx.ReadValue<float>());
        
        private void SelectAction(int num)
        {
            if (num > _current.Actions.Count)
            {
                Debug.Log($"Action{num} is not Available");
                return;
            }

            if(num -1 == _actionController.CurrentSelection) DeselectCurrentAction();
            else _actionController.Select(num - 1);
            
            GameManager.Instance.TriggerState(_current.Actions[num - 1].Type);
        }

        public void DeselectCurrentAction() => _actionController.Deselect();
        public void ActionCellPressed(int index) => SelectAction(index);
        
        private void OnDisable() => GameManager.OnCurrentChange -= OnChangeEvent;
        private void OnEnable() => GameManager.OnCurrentChange += OnChangeEvent;
        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}