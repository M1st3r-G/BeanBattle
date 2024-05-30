using System.Collections;
using Controller;
using Managers;
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
            s.path = null;
            
            AudioEffectsManager.Instance.PlayEffect(AudioEffectsManager.AudioEffect.Move);
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
                Vector2Int startCell = GridManager.Instance.WorldToCell(s.MyCharacter.transform.position);
                s.path = GridManager.Instance.GetPath(startCell, hoveredCell);
                UIManager.Instance.SetTimeCost(s.path.Length);
            }
            else
            {
                // When hovering over Enemy (cells) it its range is displayed
                GridManager.Instance.DisplayRange(GridManager.Instance.GetOccupier(hoveredCell));
            }

            if (!Mouse.current.leftButton.wasPressedThisFrame) return false;
            
            // When the Cell was Clicked, it starts the Animation
            s.MyCharacter.StartCoroutine(AnimateMovement(s));
            return true;
        }
        
        #endregion

        #region Animation

        private IEnumerator AnimateMovement(CharStateController s)
        {
            CharController.OnPlayerStartedAction?.Invoke(ActionType);
            s.IsAnimating = true;
            Vector2Int lastCell = new Vector2Int();
            
            GridManager.Instance.RenderPath(s.path);
            
            // for each field in the Path
            foreach (Vector2Int currentTargetCell in s.path)
            {
                lastCell = currentTargetCell;
                
                // Lerp position
                Vector3 startPosition = s.MyCharacter.transform.position;
                Vector3 endPosition = GridManager.Instance.CellToCenterWorld(currentTargetCell);

                float t = 0f;
                float timeForSpace = AudioEffectsManager.Instance.PlayEffect(AudioEffectsManager.AudioEffect.Steps);
                
                while (t<timeForSpace)
                {
                    s.MyCharacter.transform.position = Vector3.Lerp(startPosition, endPosition, t / timeForSpace);
                    t += Time.deltaTime;
                    yield return null;
                }

                GridManager.Instance.HideNextPathCell();
            }
            
            GridManager.Instance.SetOccupied(s.MyCharacter, lastCell);
            
            // When Finished, enable Input
            s.IsAnimating = false;
            GameManager.Instance.CountSteps(s.path.Length);
            CharController.OnPlayerFinishedAction?.Invoke(ActionType);
        }
        
        #endregion
    }
}