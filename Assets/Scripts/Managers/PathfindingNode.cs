using UnityEngine;

namespace Managers
{
    public class PathfindingNode : MonoBehaviour
    {
        public Vector2Int Position { get; private set; }
        [HideInInspector]
        public PathfindingNode parent;
        [HideInInspector]
        public bool obstacle;
        public int FCost => GCost + HCost;
        [HideInInspector]
        public int GCost, HCost;

        public void Initialize(Vector2Int position, bool isOb)
        {
            Position = position;
            obstacle = isOb;
        }
    }
}