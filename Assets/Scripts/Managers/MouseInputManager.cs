using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class MouseInputManager : MonoBehaviour
    {
        //ComponentReferences
        [SerializeField] private InputActionReference mouseClickAction;
        private Camera _mainCamera;

        public static MouseInputManager Instance { get; private set; }

        public delegate void OnCharacterClickedDelegate(CharController clickedChar);
        public static OnCharacterClickedDelegate OnCharacterClicked;
        
        private void Awake()
        {
            if (Instance is not null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;

            _mainCamera = Camera.main;
        }

        private void CustomOnMouseDown(InputAction.CallbackContext ctx)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.value);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return;
            GameObject target = hit.collider.gameObject;

            if (!target.CompareTag("Character")) return;
            
            CharController character = target.GetComponent<CharController>();
            OnCharacterClicked?.Invoke(character);
        }

        public bool GetCellFromMouse(out Vector2Int cell)
        {
            cell = new Vector2Int();
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.value);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return false;
            GameObject target = hit.collider.gameObject;

            if (!target.CompareTag("Ground")) return false;
            
            cell = (Vector2Int)GridManager.Instance.Grid.WorldToCell(hit.point);
            return true;
        }

        private void OnEnable()
        {
            mouseClickAction.action.Enable();
            mouseClickAction.action.performed += CustomOnMouseDown;
        }
        
        private void OnDisable()
        {
            mouseClickAction.action.Disable();
            mouseClickAction.action.performed -= CustomOnMouseDown;
        }

        private void OnDestroy()
        {
            if(Instance == this) Destroy(gameObject);
        }
    }
}
