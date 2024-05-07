using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using UnityEngine;

namespace Managers
{
    public class GridManager : MonoBehaviour
    {
        public Grid Grid { get; private set; }

        private Camera _mainCamera;
        private Dictionary<CharController, Vector2Int> _occupied;

        public static GridManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance is not null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;

            Grid = GetComponent<Grid>();

            _occupied = new Dictionary<CharController, Vector2Int>();
        }
        
        private void Start()
        {
            foreach (CharController charC in GameObject.FindGameObjectsWithTag("Character").Select(g => g.GetComponent<CharController>()))
            {
                _occupied[charC] = (Vector2Int)Grid.WorldToCell(charC.transform.position);
            }
        }
        
        public Vector3 CellToCenterWorld(Vector2Int cell)
        {
            return Grid.GetCellCenterWorld((Vector3Int)cell);
        }

        public void SetOccupied(CharController charController,Vector2Int cell)
        {
            if (!IsOccupied(cell))
            {
                _occupied[charController] = cell;
            }
        }

        public bool IsOccupied(Vector2Int cell)
        {
            return _occupied.ContainsValue(cell);
        }

        public Vector2Int GetPosition(CharController charController)
        {
            return _occupied[charController];
        }
        
        private void OnDestroy()
        {
            if(Instance == this) Destroy(gameObject);
        }
    }
}