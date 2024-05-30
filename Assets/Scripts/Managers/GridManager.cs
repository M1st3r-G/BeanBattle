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
        #region Fields

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

        #endregion

        #region SetUp
        private void Awake()
        {
            Instance = this;

            _allCells = new PathfindingNode[numberOfCells.x, numberOfCells.y];
            activeCells = new List<PathfindingNode>();
            _occupied = new Dictionary<CharController, Vector2Int>();
            _grid = GetComponent<Grid>();
            
            GenerateCells();
            FixPlaneTransform();
        }
        
        /// <summary>
        /// Adjusts the Size of the Plane and Moves it so that the Plane is centered in the actual Grid GO.
        /// </summary>
        private void FixPlaneTransform()
        {
            Vector3 effectiveSize = new Vector3
            {
                x = (_grid.cellSize.x + _grid.cellGap.x) * numberOfCells.x,
                y = _grid.cellSize.x * 10,
                z = (_grid.cellSize.y + _grid.cellGap.y) * numberOfCells.y
            };
            
            plane.transform.localScale = effectiveSize* 0.1f;
            transform.position -= new Vector3(effectiveSize.x / 2f, 0, effectiveSize.z / 2f);
            plane.transform.position += new Vector3(effectiveSize.x / 2f, 0, effectiveSize.z / 2f);
        }
        
        /// <summary>
        /// Generates the Cells, used to Display the Range
        /// The Cells are saved as <see cref="PathfindingNode"/> Components
        /// </summary>
        /// <seealso cref="DisplayRange"/>
        private void GenerateCells()
        {
            // Crate a reference with fitting size
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
        
        /// <summary>
        /// Sets the Starting Cell of a Player as Occupied. Also set the Characters position to that cell
        /// </summary>
        /// <param name="character">The <see cref="CharController"/> of the Character</param>
        /// <param name="startCell">The starting Cell x and y as a <see cref="Vector2Int"/></param>
        /// <param name="startHeight">The start height of the Character (Default = 1.0)</param>
        public void AddCharacter(CharController character, Vector2Int startCell, float startHeight = 1f)
        {
            _occupied[character] = startCell;
            
            Vector3 playerPosition = CellToCenterWorld(startCell);
            playerPosition.y = startHeight;
            character.transform.position = playerPosition; 
        }
        
        /// <summary>
        /// Convert a <see cref="Vector2Int"/> Cell Coordinate to the World Position
        /// </summary>
        /// <param name="cell">The Cell Coordinates</param>
        /// <returns>The <see cref="Vector3"/> World Position belonging (above) the Cell</returns>
        /// <seealso cref="WorldToCell"/>
        public Vector3 CellToCenterWorld(Vector2Int cell)
        {
            Vector3 tmp = _grid.GetCellCenterWorld((Vector3Int)cell);
            tmp.y = 1;
            return tmp;
        }

        /// <summary>
        /// Convert a <see cref="Vector3"/> World Position to the Cell Coordinate (ignores the y Value)
        /// </summary>
        /// <param name="position">The World Position</param>
        /// <returns>The x, y Coordinates as a <see cref="Vector3"/></returns>
        public Vector2Int WorldToCell(Vector3 position) => (Vector2Int)_grid.WorldToCell(position);
        
        private void RemoveDeadPlayer(CharController player) => _occupied.Remove(player);
        
        #endregion

        #region RangeManagement
        
        /// <summary>
        /// Wrapper to be Easier Called by a Character
        /// </summary>
        /// <param name="centerChar">The Character whose Range should be displayed</param>
        /// <seealso cref="DisplayRange(Vector2Int, int)"/>
        public void DisplayRange(CharController centerChar) => 
            DisplayRange(GetPosition(centerChar), centerChar.GetData.AttackRange);

        /// <summary>
        /// Displays a Range using Cells (with the <see cref="PathfindingNode"/> Component
        /// </summary>
        /// <param name="position">The center position of the Range</param>
        /// <param name="range">The Maximum Distance a cell can be from the <paramref name="position"/> to be in the Range</param>
        /// <seealso cref="DisplayRange(CharController)"/>
        private void DisplayRange(Vector2Int position, int range)
        {
            // If Cell out of Grid
            if (0 > position.x || position.x >= numberOfCells.x || 0 > position.y ||
                position.y >= numberOfCells.y) return;
            
            PathfindingNode currentCell = _allCells[position.x, position.y];
            
            // If unvisited add to Active Cells
            if (!currentCell.gameObject.activeSelf) activeCells.Add(currentCell);
            currentCell.gameObject.SetActive(true);
            
            // If Range allows, continue in each Direction
            if (range <= 0) return;
            DisplayRange(position + Vector2Int.down, range - 1);
            DisplayRange(position + Vector2Int.up, range - 1);
            DisplayRange(position + Vector2Int.left, range - 1);
            DisplayRange(position + Vector2Int.right, range - 1);
        }

        /// <summary>
        /// Selects the Characters on activeCells given by the <see cref="DisplayRange(CharController)"/>
        /// </summary>
        /// <returns>An Enumerable containing the Characters</returns>
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

        /// <summary>
        /// Used by the A* Algorithm to Determine if a Cell is Walkable
        /// </summary>
        /// <param name="node">The Node in Question</param>
        /// <returns>True if the Cell is not Walkable</returns>
        private bool IsObstacle(PathfindingNode node)
        {
            //ToDo Extend with Terrain
            return IsOccupied(node.Position);
        }

        /// <summary>
        /// Wrapper Method for easier use of the A* Algorithm
        /// </summary>
        /// <param name="start">The starting x, y coordinates</param>
        /// <param name="end">The ending x, y coordinates</param>
        /// <returns>An Enumerable containing alls Cells of the Path (Excluding the start, Including the End)</returns>
        /// <seealso cref="FindPath"/>
        public Vector2Int[] GetPath(Vector2Int start, Vector2Int end) =>
            FindPath(_allCells[start.x, start.y], _allCells[end.x, end.y]).Select(node => node.Position).ToArray();

        /// <summary>
        /// The Main A* Algorithm using <see cref="PathfindingNode"/>
        /// </summary>
        /// <param name="startN">The starting Node</param>
        /// <param name="targetN">The target Node</param>
        /// <returns>An Enumerable containing the Nodes in the Path (Excluding Start, Including End)</returns>
        /// <seealso cref="GetPath"/>
        private IEnumerable<PathfindingNode> FindPath(PathfindingNode startN, PathfindingNode targetN)
        {
            //get player and target position in this coords
            _seekerNode = startN;
            _targetNode = targetN;

            List<PathfindingNode> openSet = new List<PathfindingNode>();
            HashSet<PathfindingNode> closedSet = new HashSet<PathfindingNode>();
            openSet.Add(_seekerNode);

            // Makes it Visually more Appealing
            bool alternate = false;
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
                foreach (PathfindingNode neighbour in GetNeighbors(node, alternate))
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
                        neighbour.Parent = node;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }

                    alternate = !alternate;
                }
            }

            return null;
        }

        /// <summary>
        /// Used internally by the A* Algorithm Reverses the calculated Path using the <see cref="PathfindingNode.Parent"/> Node
        /// </summary>
        /// <param name="startNode">The startNode</param>
        /// <param name="endNode">The EndNode</param>
        /// <returns>An Enumerable Containing the Path (Exclude Start, Include End)</returns>
        /// <seealso cref="FindPath"/>
        private static IEnumerable<PathfindingNode> RetracePath(PathfindingNode startNode, PathfindingNode endNode)
        {
            List<PathfindingNode> path = new List<PathfindingNode>();
            PathfindingNode currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Reverse();

            return path;
        }

        /// <summary>
        /// Gets the (legal) neighboring nodes in the 4 cardinal directions.
        /// Optionally uses the <paramref name="alternate"/> parameter to make it visually more interesting
        /// </summary>
        /// <param name="node">The Node to find the neighbours of</param>
        /// <param name="alternate">Wheter to Reverse the Directions or not</param>
        /// <returns> An Enumerable with the Nodes</returns>
        private IEnumerable<PathfindingNode> GetNeighbors(PathfindingNode node, bool alternate = false)
        {
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            if (alternate) directions = directions.Reverse().ToArray();
            
            return from direction in directions
                   select node.Position + direction into c
                   where c.x >= 0 && c.x < numberOfCells.x && c.y >= 0 && c.y < numberOfCells.y
                   select _allCells[c.x, c.y];
        }

        /// <summary>
        /// Displays The Path
        /// </summary>
        /// <param name="path">The Path to Display</param>
        public void RenderPath(IEnumerable<Vector2Int> path)
        {
            foreach (Vector2Int cell in path)
            {
                activeCells.Add(_allCells[cell.x, cell.y]);
                _allCells[cell.x, cell.y].gameObject.SetActive(true);
            }
        }

        public void HideNextPathCell()
        {
            PathfindingNode cell = activeCells[0];
            activeCells.RemoveAt(0);
            cell.gameObject.SetActive(false);
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
