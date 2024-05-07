using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class MouseInputManager : MonoBehaviour
    {
        //ComponentReferences
        private Camera _mainCamera;

        public static MouseInputManager Instance { get; private set; }
        
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
        
        private void OnDestroy()
        {
            if(Instance == this) Destroy(gameObject);
        }
    }
}
