using System.Collections.Generic;
using System.Linq;
using Controller;
using UnityEngine;

namespace Managers
{
    public class GridManager : MonoBehaviour
    {
        //Components
        public Grid Grid { get; private set; }
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private GameObject cellContainer;
        private Camera _mainCamera;
        [SerializeField] private GameObject plane;
        
        //Params
        [SerializeField] private Vector2Int numberOfCells;
        private GameObject[,] allCells;
        
        //Temps
        private Dictionary<CharController, Vector2Int> _occupied;
        public List<GameObject> activeCells;
        
        //Publics
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

            allCells = new GameObject[numberOfCells.x, numberOfCells.y];
            activeCells = new List<GameObject>();
            _occupied = new Dictionary<CharController, Vector2Int>();
            Grid = GetComponent<Grid>();
            
            GenerateCells();
            FixPlaneTransform();
        }
        
        private void Start()
        {
            foreach (CharController charC in GameObject.FindGameObjectsWithTag("Character").Select(g => g.GetComponent<CharController>()))
            {
                Vector2Int pos = (Vector2Int)Grid.WorldToCell(charC.transform.position);
                _occupied[charC] = pos;
                Vector3 playerPos = CellToCenterWorld(pos);
                playerPos.y = charC.transform.position.y;
                charC.transform.position = playerPos; 
            }
        }
        
        private void FixPlaneTransform()
        {
            Vector3 efficientSize = new Vector3
            {
                x = (Grid.cellSize.x + Grid.cellGap.x) * numberOfCells.x,
                y = Grid.cellSize.x * 10,
                z = (Grid.cellSize.y + Grid.cellGap.y) * numberOfCells.y
            };
            plane.transform.localScale = efficientSize* 0.1f;
            transform.position -= new Vector3(efficientSize.x / 2f, 0, efficientSize.z / 2f);
            plane.transform.position += new Vector3(efficientSize.x / 2f, 0, efficientSize.z / 2f);
        }
        
        private void GenerateCells()
        {
            GameObject reference = Instantiate(cellPrefab);
            reference.transform.localScale =
                new Vector3(Grid.cellSize.x, reference.transform.localScale.y, Grid.cellSize.y);
            
            for (int x = 0; x < numberOfCells.x; x++)
            {
                for (int y = 0; y < numberOfCells.y; y++)
                {
                    Vector3 position = Grid.GetCellCenterWorld(new Vector3Int(x, y));
                    position.y = 0;
                    allCells[x,y] = Instantiate(reference, position , Quaternion.identity, cellContainer.transform);
                    allCells[x,y].SetActive(false);
                }
            }
            
            Destroy(reference);
        }
        
        public void DisplayRange(CharController centerChar, int range)
        {
            DisplayRange(GetPosition(centerChar), range);
        }

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

        public int CharactersInRange(out CharController[] lastCharacter)
        {
            List<CharController> charList = new();
            int counter = 0;
            foreach (GameObject cell in activeCells)
            {
                Vector2Int coordinate = (Vector2Int)Grid.WorldToCell(cell.transform.position);
                Debug.Log(coordinate);
                if (!IsOccupied(coordinate)) continue;
                counter++;
                charList.Add(GetOccupier(coordinate));
            }

            lastCharacter = charList.ToArray();
            return counter;
        }

        private CharController GetOccupier(Vector2Int cell) => _occupied.FirstOrDefault(x => x.Value == cell).Key;

        public void ResetRange()
        {
            for (int i = activeCells.Count - 1; i >= 0; i--)
            {
                activeCells[i].gameObject.SetActive(false);
                activeCells.RemoveAt(i);
            }
        }
        
        public void SetOccupied(CharController charController,Vector2Int cell)
        {
            if (!IsOccupied(cell)) _occupied[charController] = cell;
        }
        public bool IsOccupied(Vector2Int cell) => _occupied.ContainsValue(cell);
        public Vector2Int GetPosition(CharController charController) => _occupied[charController];
        public Vector3 CellToCenterWorld(Vector2Int cell) => Grid.GetCellCenterWorld((Vector3Int)cell);
        
        private void OnDestroy()
        {
            if(Instance == this) Destroy(gameObject);
        }
    }
}
