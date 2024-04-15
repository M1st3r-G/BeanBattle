using Data;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Controller
{
    public class CurrentCharacterUIController : MonoBehaviour
    {
        private ActionsUIController _actionController;
        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI nameText; 
        [SerializeField] private InputActionReference numberAction;
        private CharData _current;
        
        private void Awake()
        {
            _actionController = GetComponentInChildren<ActionsUIController>();
        }

        private void OnChangeEvent(CharController newChar)
        {
            _current = newChar.GetData;
            _actionController.SetDisplay(_current.Actions);
            portrait.color = _current.Material.color;
            nameText.text = _current.Name;
        }

        private void OnEnable()
        {
            GameManager.OnCurrentChange += OnChangeEvent;
            numberAction.action.Enable();
            numberAction.action.performed += ExecuteAction;
        }

        private void ExecuteAction(InputAction.CallbackContext ctx)
        {
            int num = (int) ctx.ReadValue<float>();
            if(num > _current.Actions.Count) print($"Action{num} is not Available");
            else{print($"Executing the {_current.Actions[num-1].ActionName} action");}
        }

        private void OnDisable()
        {
            GameManager.OnCurrentChange -= OnChangeEvent;
            numberAction.action.performed -= ExecuteAction;
            numberAction.action.Disable();

        }
    }
}