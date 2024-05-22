using System.Collections;
using System.Collections.Generic;
using Controller;
using Managers;
using Misc;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "Move", menuName = "States/Move", order = 10)]
    public class CharacterMoveState : CharacterStateBase
    {
        #region DefaultStateMethods
        
        public override void StateSetUp(CharStateController s) => s.MyCharacter.Indicator.SetActive(true);
        public override void StateDisassembly(CharStateController s) => s.MyCharacter.Indicator.SetActive(false);
        
        public override bool ExecuteStateFrame(CharStateController s)
        {
            if (!MouseInputManager.Instance.GetCellFromMouse(out Vector2Int hoveredCell)) return false;
            
            if (!GridManager.Instance.IsOccupied(hoveredCell))
            {
                GridManager.Instance.ResetRange();
                Vector3 newPosition = GridManager.Instance.CellToCenterWorld(hoveredCell);
                newPosition.y = s.MyCharacter.transform.position.y;
                s.MyCharacter.Indicator.transform.position = newPosition;
                        
                int timeCost = GridManager.Instance.GetPosition(s.MyCharacter).ManhattanDistance(hoveredCell);
                CurrentActionController.Instance.SetTimeCost(timeCost);
            }
            else
            {
                GridManager.Instance.DisplayRange(GridManager.Instance.GetOccupier(hoveredCell));
            }

            if (!Mouse.current.leftButton.wasPressedThisFrame) return false;
            
            s.MyCharacter.StartCoroutine(AnimateMovement(s, 
                GridManager.Instance.GetPath(GridManager.Instance.WorldToCell(s.MyCharacter.transform.position),
                    hoveredCell)));
            
            GridManager.Instance.SetOccupied(s.MyCharacter, hoveredCell);
            return true;
        }
        
        #endregion

        #region Animation

        private static IEnumerator AnimateMovement(CharStateController s, IReadOnlyList<Vector2Int> path)
        {
            int currentPathIndex = 0;

            while (currentPathIndex < path.Count)
            {
                var startPosition = s.MyCharacter.transform.position;
                var endPosition = GridManager.Instance.CellToCenterWorld(path[currentPathIndex]);

                float t = 0f;
                
                while (t<s.TimePerSpace)
                {
                    s.MyCharacter.transform.position = Vector3.Lerp(startPosition, endPosition, t / s.TimePerSpace);
                    t += Time.deltaTime;
                    yield return null;
                }

                currentPathIndex++;
            }
        }
        public override void OnPlayerDeath(CharStateController s, CharController deadPlayer) { }
        #endregion
    }
}