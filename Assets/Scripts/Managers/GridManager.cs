using System.Collections.Generic;
using System.Linq;
using Controller;
using Data;
using Misc;
using UnityEngine;

namespace Managers
{
    public class GridManager : MonoBehaviour
    {
        //Components
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private GameObject cellContainer;
        [SerializeField] private GameObject plane;
        private Grid _grid;
        private Camera _mainCamera;

        //Params
        [SerializeField] private Vector2Int numberOfCells;
        private PathfindingNode[,] _allCells;
        
        //Temps
        private Dictionary<CharController, Vector2Int> _occupied;
        public List<PathfindingNode> activeCells;
        private PathfindingNode _seekerNode, _targetNode;
        
        //Public
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

            _allCells = new PathfindingNode[numberOfCells.x, numberOfCells.y];
            activeCells = new List<PathfindingNode>();
            _occupied = new Dictionary<CharController, Vector2Int>();
            _grid = GetComponent<Grid>();
            
            GenerateCells();
            FixPlaneTransform();
        }
        
        private void FixPlaneTransform()
        {
            Vector3 efficientSize = new Vector3
            {
                x = (_grid.cellSize.x + _grid.cellGap.x) * numberOfCells.x,
                y = _grid.cellSize.x * 10,
                z = (_grid.cellSize.y + _grid.cellGap.y) * numberOfCells.y
            };
            plane.transform.localScale = efficientSize* 0.1f;
            transform.position -= new Vector3(efficientSize.x / 2f, 0, efficientSize.z / 2f);
            plane.transform.position += new Vector3(efficientSize.x / 2f, 0, efficientSize.z / 2f);
        }
        
        private void GenerateCells()
        {
            GameObject reference = Instantiate(cellPrefab);
            reference.transform.localScale =
                new Vector3(_grid.cellSize.x, reference.transform.localScale.y, _grid.cellSize.y);
            
            for (int x = 0; x < numberOfCells.x; x++)
            {
                for (int y = 0; y < numberOfCells.y; y++)
                {
                    Vector3 position = _grid.GetCellCenterWorld(new Vector3Int(x, y));
                    position.y = 0;
                    PathfindingNode newNode = Instantiate(reference, position , Quaternion.identity, cellContainer.transform).GetComponent<PathfindingNode>();
                    newNode.gameObject.SetActive(false);
                    newNode.Initialize(new Vector2Int(x, y));
                    _allCells[x, y] = newNode;
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
        
        public Vector3 CellToCenterWorld(Vector2Int cell)
        {
            Vector3 tmp = _grid.GetCellCenterWorld((Vector3Int)cell);
            tmp.y = 1;
            return tmp;
        }

        public Vector2Int WorldToCell(Vector3 position) => (Vector2Int)_grid.WorldToCell(position);
        private void RemoveDeadPlayer(CharController player) => _occupied.Remove(player);
        #endregion

        #region RangeManagement
        public void DisplayRange(CharController centerChar) => 
            DisplayRange(GetPosition(centerChar), centerChar.GetData.AttackRange);

        private void DisplayRange(Vector2Int position, int range)
        {
            if (0 > position.x || position.x >= numberOfCells.x || 0 > position.y ||
                position.y >= numberOfCells.y) return;
            
            PathfindingNode currentCell = _allCells[position.x, position.y];
            
            if (!currentCell.gameObject.activeSelf) activeCells.Add(currentCell);
            currentCell.gameObject.SetActive(true);
            
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
            foreach (PathfindingNode cell in activeCells) cell.gameObject.SetActive(false);
            activeCells.Clear();
        }
        #endregion
        
        #region PathManagement

        private bool IsObstacle(PathfindingNode node)
        {
            return IsOccupied(node.Position);
        }
        
        public Vector2Int[] GetPath(Vector2Int start, Vector2Int end) => FindPath(_allCells[start.x, start.y], _allCells[end.x, end.y]).Select(node => node.Position).ToArray();
        
        private IEnumerable<PathfindingNode> FindPath(PathfindingNode startN, PathfindingNode targetN)
        {
            //get player and target position in this coords
            _seekerNode = startN;
            _targetNode = targetN;

            List<PathfindingNode> openSet = new List<PathfindingNode>();
            HashSet<PathfindingNode> closedSet = new HashSet<PathfindingNode>();
            openSet.Add(_seekerNode);
        
            //calculates path for pathfinding
            while (openSet.Count > 0)
            {

                //iterates through openSet and finds lowest FCost
                PathfindingNode node = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost <= node.FCost)
                    {
                        if (openSet[i].HCost < node.HCost)
                            node = openSet[i];
                    }
                }

                openSet.Remove(node);
                closedSet.Add(node);

                //If target found, retrace path
                if (node == _targetNode)
                {
                    return RetracePath(_seekerNode, _targetNode);
                }
            
                //adds neighbor nodes to openSet
                foreach (PathfindingNode neighbour in GetNeighbors(node))
                {
                    if (IsObstacle(neighbour) || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newCostToNeighbour = node.GCost + node.Position.ManhattanDistance(neighbour.Position);
                    if (newCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newCostToNeighbour;
                        neighbour.HCost = neighbour.Position.ManhattanDistance(_targetNode.Position);
                        neighbour.parent = node;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            return null;
        }

        //reverses calculated path so first node is closest to seeker
        private static IEnumerable<PathfindingNode> RetracePath(PathfindingNode startNode, PathfindingNode endNode)
        {
            List<PathfindingNode> path = new List<PathfindingNode>();
            PathfindingNode currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();

            return path;
        }
    
        //gets the neighboring nodes in the 4 cardinal directions. If you would like to enable diagonal pathfinding, uncomment out that portion of code
        private List<PathfindingNode> GetNeighbors(PathfindingNode node) =>
            (from direction in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right }
                select node.Position + direction into c
                where c.x >= 0 && c.x < numberOfCells.x && c.y >= 0 && c.y < numberOfCells.y
                select _allCells[c.x, c.y]).ToList();

        public void RenderPath(Vector2Int[] path)
        {
            foreach (Vector2Int cell in path)
            {
                _allCells[cell.x, cell.y].gameObject.SetActive(true);
            }
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
