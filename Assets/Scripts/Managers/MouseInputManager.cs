using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class MouseInputManager : MonoBehaviour
    {
        //ComponentReferences
        private Grid _grid;
        private Camera _mainCamera;
        private List<Vector2Int> _occupied;

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

        public void SetOccupied(Vector3Int cell)
        {
            if (!IsOccupied(cell))
            {
                _occupied.Add((Vector2Int)cell);
            }
            else throw new Exception("Cell already full");
        }

        public bool IsOccupied(Vector3Int cell)
        {
            return _occupied.Contains((Vector2Int)cell);
        }

        private void OnDestroy()
        {
            if(Instance == this) Destroy(gameObject);
        }
    }
}
