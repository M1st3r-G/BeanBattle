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
        public override bool ExecuteStateFrame()
        {
            if(MouseInputManager.Instance.GetCellFromMouse(out Vector2Int hoveredCell))
            {
                if (!GridManager.Instance.IsOccupied(hoveredCell))
                {
                    Vector3 newPosition = GridManager.Instance.CellToCenterWorld(hoveredCell);
                    newPosition.y = ActiveCharacter.transform.position.y;
                    ActiveCharacter.Indicator.transform.position = newPosition;
                        
                    int timeCost = GridManager.Instance.GetPosition(ActiveCharacter).ManhattanDistance(hoveredCell);
                    CurrentActionController.Instance.SetTimeCost(timeCost);
                }

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Debug.Log("Accepted");
                    ActiveCharacter.transform.position = ActiveCharacter.Indicator.transform.position;
                    GridManager.Instance.SetOccupied(ActiveCharacter, hoveredCell);
                    return true;
                }
            }

            return false;
        }
        
        public override void InternalStateSetUp()
        {
            ActiveCharacter.Indicator.SetActive(true);
        }

        public override void StateDisassembly()
        {
            ActiveCharacter.Indicator.SetActive(false);
        }
    }
}