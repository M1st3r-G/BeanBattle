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

        public Vector3Int? GetCellFromMouse()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.value);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return null;
            GameObject target = hit.collider.gameObject;

            if (target.CompareTag("Ground")) return GridManager.Instance.Grid.WorldToCell(hit.point);
            return null;
        }
        
        private void OnDestroy()
        {
            if(Instance == this) Destroy(gameObject);
        }
    }
}
