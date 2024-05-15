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

        // Public
        public static MouseInputManager Instance { get; private set; }

        // Events
        public delegate void OnCharacterClickedDelegate(CharController clickedChar);
        public static OnCharacterClickedDelegate OnCharacterClicked;

        #region SetUp
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
        #endregion

        #region MainMethods
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

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground"))) return false;
            GameObject target = hit.collider.gameObject;

            if (!target.CompareTag("Ground")) return false;
            
            cell = GridManager.Instance.WorldToCell(hit.point);
            return true;
        }
        #endregion
    }
}
