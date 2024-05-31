using System.Collections;
using Controller;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "Move", menuName = "States/Move", order = 10)]
    public class CharacterMoveState : CharacterStateBase
    {
        public override void StateSetUp(CharStateController s)
        {
            // Set State Variables
            s.MyCharacter.Indicator.SetActive(true);
            s.path = null;
            
            // Play Audio
            AudioEffectsManager.Instance.PlayEffect(AudioEffectsManager.AudioEffect.Move);
        }

        public override void StateDisassembly(CharStateController s)
        {
            s.MyCharacter.Indicator.SetActive(false);
        }

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

        protected override IEnumerator ExecuteStateAndAnimate(CharStateController s)
        {
            CustomInputManager.Instance.DisableInput();
            
            // for each field in the Path
            foreach (Vector2Int currentTargetCell in s.path)
            {
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
            
            GridManager.Instance.SetOccupied(s.MyCharacter,  s.path[^1]);
            
            GameManager.Instance.CountSteps(s.path.Length);
            
            // When Finished, Call Method
            GameManager.Instance.FullActionEnd();
        }
    }
}