using System.Collections;
using Controller;
using Data;
using Data.CharacterStates;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tutorial
{
    public class TutorialMovementState : CharacterMoveState
    {
        public override bool ExecuteStateFrame(CharStateController s)
        {
            // If(Input) ChangeState
            if (!MouseInputManager.Instance.GetCellFromMouse(out Vector2Int hoveredCell)) return false;
            if (!GridManager.Instance.IsOccupied(hoveredCell))
            {
                // Stop showing Enemy's Ranges
                GridManager.Instance.ResetRange();
                
                // Display new Position with the Indicator
                Vector3 newPosition = GridManager.Instance.CellToCenterWorld(hoveredCell);
                newPosition.y = s.MyCharacter.transform.position.y;
                s.MyCharacter.Indicator.transform.position = newPosition;
                
                // Calculates and Renders Path
                Vector2Int startCell = GridManager.Instance.WorldToCell(s.MyCharacter.transform.position);
                s.path = GridManager.Instance.GetPath(startCell, hoveredCell);
                GridManager.Instance.RenderPath(s.path);
                
                // Adjusts the Action Cost
                UIManager.Instance.SetTimeCost(s.path.Length);
            }
            else
            {
                // When hovering over Enemy (cells) it its range is displayed
                GridManager.Instance.ResetRange();
                GridManager.Instance.DisplayRange(GridManager.Instance.GetOccupier(hoveredCell));
            }

            // If(Accept)
            if (!Mouse.current.leftButton.wasPressedThisFrame) return false;
            s.MyCharacter.StartCoroutine(ExecuteStateAndAnimate(s));
            return true;
        }
    }
}