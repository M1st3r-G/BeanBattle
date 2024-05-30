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
            Instance = this;

            _mainCamera = Camera.main;
        }

        private void OnDestroy()
        {
            if(Instance == this) Destroy(gameObject);
        }
        
        #endregion

        #region MainMethods
        
        /// <summary>
        /// When Triggered it RayTraces for the First Character it Sees at Mouse Position
        /// Should Only be Called after A Click
        /// </summary>
        /// <param name="clickedChar">The Character Clicked on or null</param>
        /// <returns>True if a Character was Clicked</returns>
        public bool GetCharacterClicked(out CharController clickedChar)
        {
            clickedChar = null;
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.value);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return false;
            GameObject target = hit.collider.gameObject;

            if (!target.CompareTag("Character")) return false;
            
            clickedChar = target.GetComponent<CharController>();
            return true;
        }
        
        /// <summary>
        /// When Triggered, it RayTraces (in the Ground Layer) for the Cell at the Mouse Position
        /// </summary>
        /// <param name="cell">The cell the Mouse is over</param>
        /// <returns>True if the Mouse is over a cell</returns>
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
