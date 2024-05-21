using System.Collections.Generic;
using System.Linq;
using Controller;
using UnityEngine;

namespace Managers
{
    public class GridManager : MonoBehaviour
    {
        //Components
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private GameObject cellContainer;
        [SerializeField] private GameObject plane;
        private Grid grid;
        private Camera _mainCamera;
        
        //Params
        [SerializeField] private Vector2Int numberOfCells;
        private GameObject[,] allCells;
        
        //Temps
        private Dictionary<CharController, Vector2Int> _occupied;
        public List<GameObject> activeCells;
        
        //Publics
        public static GridManager Instance { get; private set; }

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

            allCells = new GameObject[numberOfCells.x, numberOfCells.y];
            activeCells = new List<GameObject>();
            _occupied = new Dictionary<CharController, Vector2Int>();
            grid = GetComponent<Grid>();
            
            GenerateCells();
            FixPlaneTransform();
        }
        
        private void FixPlaneTransform()
        {
            Vector3 efficientSize = new Vector3
            {
                x = (grid.cellSize.x + grid.cellGap.x) * numberOfCells.x,
                y = grid.cellSize.x * 10,
                z = (grid.cellSize.y + grid.cellGap.y) * numberOfCells.y
            };
            plane.transform.localScale = efficientSize* 0.1f;
            transform.position -= new Vector3(efficientSize.x / 2f, 0, efficientSize.z / 2f);
            plane.transform.position += new Vector3(efficientSize.x / 2f, 0, efficientSize.z / 2f);
        }
        
        private void GenerateCells()
        {
            GameObject reference = Instantiate(cellPrefab);
            reference.transform.localScale =
                new Vector3(grid.cellSize.x, reference.transform.localScale.y, grid.cellSize.y);
            
            for (int x = 0; x < numberOfCells.x; x++)
            {
                for (int y = 0; y < numberOfCells.y; y++)
                {
                    Vector3 position = grid.GetCellCenterWorld(new Vector3Int(x, y));
                    position.y = 0;
                    allCells[x,y] = Instantiate(reference, position , Quaternion.identity, cellContainer.transform);
                    allCells[x,y].SetActive(false);
                }
            }
            
            Destroy(reference);
        }
        
        private void OnDestroy()
        {
            if(Instance == this) Destroy(gameObject);
        }
        
        private void OnEnable() => CharController.OnPlayerDeath += RemoveDeadPlayer;
        private void OnDisable() => CharController.OnPlayerDeath -= RemoveDeadPlayer;
        #endregion

        #region OtherMethods
        public void AddCharacter(CharController character, Vector2Int startCell, float startHeight = 1f)
        {
            _occupied[character] = startCell;
            
            Vector3 playerPosition = CellToCenterWorld(startCell);
            
            playerPosition.y = startHeight;
            character.transform.position = playerPosition; 
        }
        
        public Vector3 CellToCenterWorld(Vector2Int cell) => grid.GetCellCenterWorld((Vector3Int)cell);
        public Vector2Int WorldToCell(Vector3 position) => (Vector2Int)grid.WorldToCell(position);
        private void RemoveDeadPlayer(CharController player) => _occupied.Remove(player);
        #endregion

        #region RangeManagement
        public void DisplayRange(CharController centerChar) => 
            DisplayRange(GetPosition(centerChar), centerChar.GetData.AttackRange);

        private void DisplayRange(Vector2Int position, int range)
        {
            if (0 > position.x || position.x >= numberOfCells.x || 0 > position.y ||
                position.y >= numberOfCells.y) return;
            
            GameObject currentCell = allCells[position.x, position.y];
            
            if (!currentCell.activeSelf) activeCells.Add(currentCell);
            currentCell.SetActive(true);
            
            if (range <= 0) return;
            DisplayRange(position + Vector2Int.down, range - 1);
            DisplayRange(position + Vector2Int.up, range - 1);
            DisplayRange(position + Vector2Int.left, range - 1);
            DisplayRange(position + Vector2Int.right, range - 1);
        }

        public IEnumerable<CharController> CharactersInRange() =>
            from cell in activeCells
            select WorldToCell(cell.transform.position) into coordinate
            where IsOccupied(coordinate) select GetOccupier(coordinate);
        

        public void ResetRange()
        {
            foreach (GameObject cell in activeCells) cell.SetActive(false);
            activeCells.Clear();
        }
        #endregion

        #region OccupationManagement
        public void SetOccupied(CharController charController,Vector2Int cell)
        {
            if (!IsOccupied(cell)) _occupied[charController] = cell;
        }
        
        public CharController GetOccupier(Vector2Int cell) => _occupied.FirstOrDefault(x => x.Value == cell).Key;
        public bool IsOccupied(Vector2Int cell) => _occupied.ContainsValue(cell);
        public Vector2Int GetPosition(CharController charController) => _occupied[charController];
        #endregion
    }
}
