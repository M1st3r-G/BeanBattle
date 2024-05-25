using UnityEngine;

namespace Data
{
    /// <summary>
    /// This Class is Only Used internally by the Grid
    /// <list type="number">
    /// <listheader>
    ///     <description>It has 2 Purposes</description>
    /// </listheader>
    /// <item>Saves the Data Required for the A* Algorithm</item>
    /// <item>Be a MonoBehaviour to Detect "Cells" used for the RangeDisplay.</item>
    /// </list>
    /// </summary>
    public class PathfindingNode : MonoBehaviour
    {
        public Vector2Int Position { get; private set; }
        public PathfindingNode Parent { get; set; }
        public int FCost => GCost + HCost;
        public int GCost { get; set; }
        public int HCost { get; set; }

        public void Initialize(Vector2Int position)
        {
            Position = position;
        }
    }
}