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
        private Coroutine _currentAction;
        
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
            portrait.color = _current.Material.color;
            nameText.text = _current.Name;
        }

        private void OnEnable()
        {
            GameManager.OnCurrentChange += OnChangeEvent;
            numberAction.action.Enable();
            numberAction.action.performed += NumberPressed;
        }

        private void NumberPressed(InputAction.CallbackContext ctx)
        {
            SelectAction((int)ctx.ReadValue<float>());
        }
        
        private void SelectAction(int num)
        {
            if (num > _current.Actions.Count)
            {
                print($"Action{num} is not Available");
                return;
            }

            print($"Description: {_current.Actions[num-1].ActionDescription}");
            if(_currentAction is not null) StopCoroutine(_currentAction);
            
            switch (_current.Actions[num].Action)
            {
                //THIS IS NOT THE PLACE FOR THIS TO BE
            }
        }

        public void ActionCellPressed(int index) => SelectAction(index);
        
        private void OnDisable()
        {
            GameManager.OnCurrentChange -= OnChangeEvent;
            numberAction.action.performed -= NumberPressed;
            numberAction.action.Disable();

        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}