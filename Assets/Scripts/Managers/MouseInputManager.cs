using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class MouseInputManager : MonoBehaviour
    {
        //ComponentReferences
        private Camera _mainCamera;

        // Public
        public static MouseInputManager Instance { get; private set; }

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

        private void OnDestroy()
        {
            if(Instance == this) Destroy(gameObject);
        }
        
        #endregion

        #region MainMethods
        public bool GetCharacterClicked(out CharController clickedChar)
        {
            clickedChar = null;
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.value);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return false;
            GameObject target = hit.collider.gameObject;

            if (!target.CompareTag("Character")) return false;
            
            CharController character = target.GetComponent<CharController>();
            clickedChar = character;
            return true;
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
