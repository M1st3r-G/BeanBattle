using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class MouseInputManager : MonoBehaviour
    {
        //ComponentReferences
        private Grid _grid;
        private Camera _mainCamera;
        private Dictionary<CharController, Vector2Int> _occupied;

        private Coroutine testVar;
        
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

            _grid = GameObject.FindGameObjectWithTag("Ground").GetComponent<Grid>();
            _mainCamera = Camera.main;

            _occupied = new Dictionary<CharController, Vector2Int>();
        }

        private void Start()
        {
            foreach (CharController charC in GameObject.FindGameObjectsWithTag("Character").Select(g => g.GetComponent<CharController>()))
            {
                _occupied[charC] = (Vector2Int)_grid.WorldToCell(charC.transform.position);
            }
        }

        public Vector3Int? GetCellFromMouse()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.value);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return null;
            GameObject target = hit.collider.gameObject;

            if (target.CompareTag("Ground")) return _grid.WorldToCell(hit.point);
            return null;
        }

        public Vector3 CellToCenterWorld(Vector3Int cell)
        {
            return _grid.GetCellCenterWorld(cell);
        }

        public void SetOccupied(CharController charController,Vector3Int cell)
        {
            if (!IsOccupied(cell))
            {
                _occupied[charController] = (Vector2Int)cell;
            }
            else throw new Exception("Cell already full");
        }

        public bool IsOccupied(Vector3Int cell)
        {
            return _occupied.ContainsValue((Vector2Int)cell);
        }

        private void OnDestroy()
        {
            if(Instance == this) Destroy(gameObject);
        }
    }
}
