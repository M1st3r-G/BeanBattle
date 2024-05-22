using System.Collections;
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
        [SerializeField] private float timePerSpace;

        #region DefaultStateMethods
        protected override void InternalStateSetUp() => ActiveCharacter.Indicator.SetActive(true);
        public override void StateDisassembly() => ActiveCharacter.Indicator.SetActive(false);
        
        public override bool ExecuteStateFrame()
        {
            if (!MouseInputManager.Instance.GetCellFromMouse(out Vector2Int hoveredCell)) return false;
            
            if (!GridManager.Instance.IsOccupied(hoveredCell))
            {
                GridManager.Instance.ResetRange();
                Vector3 newPosition = GridManager.Instance.CellToCenterWorld(hoveredCell);
                newPosition.y = ActiveCharacter.transform.position.y;
                ActiveCharacter.Indicator.transform.position = newPosition;
                        
                int timeCost = GridManager.Instance.GetPosition(ActiveCharacter).ManhattanDistance(hoveredCell);
                CurrentActionController.Instance.SetTimeCost(timeCost);
            }
            else
            {
                GridManager.Instance.DisplayRange(GridManager.Instance.GetOccupier(hoveredCell));
            }

            if (!Mouse.current.leftButton.wasPressedThisFrame) return false;
            
            ActiveCharacter.StartCoroutine(AnimateMovement(
                GridManager.Instance.GetPath(GridManager.Instance.WorldToCell(ActiveCharacter.transform.position),
                    hoveredCell)));
            
            GridManager.Instance.SetOccupied(ActiveCharacter, hoveredCell);
            return true;
        }
        
        #endregion

        #region Animation

        private IEnumerator AnimateMovement(Vector2Int[] path)
        {
            int currentPathIndex = 0;

            while (currentPathIndex < path.Length)
            {
                var startPosition = ActiveCharacter.transform.position;
                var endPosition = GridManager.Instance.CellToCenterWorld(path[currentPathIndex]);

                float t = 0f;
                
                while (t<timePerSpace)
                {
                    ActiveCharacter.transform.position = Vector3.Lerp(startPosition, endPosition, t / timePerSpace);
                    t += Time.deltaTime;
                    yield return null;
                }

                currentPathIndex++;
            }
        }

        #endregion
    }
}