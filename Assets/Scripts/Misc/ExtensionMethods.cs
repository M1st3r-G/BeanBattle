using System.Linq;
using UnityEngine;

namespace Misc
{
    public static class ExtensionMethods
    {
        public static int ManhattanDistance(this Vector2Int a, Vector2Int b){
            checked {
                return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
            }
        }

        public static string CustomToString(this Vector2Int[] list)
        {
            return list.Aggregate("", (old, pos) => old + pos + " | ");
        }
    }
   
}