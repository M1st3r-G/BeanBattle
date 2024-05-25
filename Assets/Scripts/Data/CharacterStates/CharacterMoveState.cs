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
        
        public override void StateSetUp(CharStateController s)
        {
            // Set State Variables
            s.IsAnimating = false;
            s.MyCharacter.Indicator.SetActive(true);
        }

        public override void StateDisassembly(CharStateController s)
        {
            //Enables Input if State is switched Away
            s.MyCharacter.Indicator.SetActive(false);
        }

        public override bool ExecuteStateFrame(CharStateController s)
        {
            if (!MouseInputManager.Instance.GetCellFromMouse(out Vector2Int hoveredCell)) return false;
            
            if (!GridManager.Instance.IsOccupied(hoveredCell))
            {
                // Stop showing Enemys Ranges
                GridManager.Instance.ResetRange();
                
                // Display new Position with the Indicator
                Vector3 newPosition = GridManager.Instance.CellToCenterWorld(hoveredCell);
                newPosition.y = s.MyCharacter.transform.position.y;
                s.MyCharacter.Indicator.transform.position = newPosition;
                
                // Adjusts the Action Cost
                int timeCost = GridManager.Instance.GetPosition(s.MyCharacter).ManhattanDistance(hoveredCell);
                UIManager.Instance.SetTimeCost(timeCost);
            }
            else
            {
                // When hovering over Enemy (cells) it its range is displayed
                GridManager.Instance.DisplayRange(GridManager.Instance.GetOccupier(hoveredCell));
            }

            if (!Mouse.current.leftButton.wasPressedThisFrame) return false;
            
            // When the Cell was Clicked, it starts the Animation
            Vector2Int startCell = GridManager.Instance.WorldToCell(s.MyCharacter.transform.position);
            IEnumerable<Vector2Int> path = GridManager.Instance.GetPath(startCell, hoveredCell);
            s.MyCharacter.StartCoroutine(AnimateMovement(s, path));
            return true;
        }
        
        #endregion

        #region Animation

        private IEnumerator AnimateMovement(CharStateController s, IEnumerable<Vector2Int> path)
        {
            CharController.OnPlayerStartedAction?.Invoke(ActionType);
            s.IsAnimating = true;
            Vector2Int lastCell = new Vector2Int();
            
            // for each field in the Path
            foreach (Vector2Int currentTargetCell in path)
            {
                lastCell = currentTargetCell;
                
                // Lerp position
                Vector3 startPosition = s.MyCharacter.transform.position;
                Vector3 endPosition = GridManager.Instance.CellToCenterWorld(currentTargetCell);

                float t = 0f;
                
                while (t<s.TimePerSpace)
                {
                    s.MyCharacter.transform.position = Vector3.Lerp(startPosition, endPosition, t / s.TimePerSpace);
                    t += Time.deltaTime;
                    yield return null;
                }
            }
            
            GridManager.Instance.SetOccupied(s.MyCharacter, lastCell);
            
            // When Finished, enable Input
            s.IsAnimating = false;
            s.EndOfStateReached(ActionType);
        }
        
        #endregion
    }
}