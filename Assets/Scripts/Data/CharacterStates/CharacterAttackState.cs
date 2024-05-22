using System.Collections.Generic;
using System.Linq;
using Controller;
using Managers;
using UnityEngine;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "Attack", menuName = "States/Attack", order = 10)]
    public class CharacterAttackState : CharacterStateBase
    {
        #region DefaultStateMethods

        public override void StateSetUp(CharStateController s)
        {
            CharController.OnPlayerStartedAction?.Invoke(ActionType);
            
            s.CurrentSelection = null;
            s.LookingForPlayer = false;
            s.AcceptAction.action.Enable();
            
            GridManager.Instance.DisplayRange(s.MyCharacter);
            CharController[] enemiesInRange = GetEnemiesInRange(s, GridManager.Instance.CharactersInRange());

            if (enemiesInRange.Length == 1) SetSelection(s, enemiesInRange[0]);
            else s.LookingForPlayer = true;
        }
        
        public override void StateDisassembly(CharStateController s)
        {
            CharController.OnPlayerFinishedAction?.Invoke(ActionType);
            s.AcceptAction.action.Disable();
            GridManager.Instance.ResetRange();
            s.LookingForPlayer = false;
            
            if (s.CurrentSelection is not null) s.CurrentSelection.SetSelector(false);
            s.CurrentSelection = null;
        }
        
        public override bool ExecuteStateFrame(CharStateController s)
        {
            if (s.LookingForPlayer)
            {
                if (s.MouseClickAction.WasPerformedThisFrame())
                {
                    if (MouseInputManager.Instance.GetCharacterClicked(out CharController clickedChar))
                    {
                        SelectClickedCharacter(s, clickedChar);
                    }
                }
            }
            
            if(s.CurrentSelection is null) return false;
            if (!s.AcceptAction.action.WasPerformedThisFrame()) return false;
            
            s.MyCharacter.PerformAttack(s.CurrentSelection);
            return true;
        }
        
        public override void OnPlayerDeath(CharStateController s, CharController deadPlayer) => RemoveDeadSelection(s, deadPlayer);

        #endregion

        #region StateMethods
        private static void SetSelection(CharStateController s, CharController targetChar)
        {
            if (s.CurrentSelection is not null) s.CurrentSelection.SetSelector(false);

            s.CurrentSelection = targetChar;
            targetChar.SetSelector(true);
        }
        
        private static void SelectClickedCharacter(CharStateController s, CharController hitCharacter)
        {
            if (hitCharacter.TeamID != s.MyCharacter.TeamID) SetSelection(s, hitCharacter);
        }
        
        private static void RemoveDeadSelection(CharStateController s, CharController c)
        {
            if (s.CurrentSelection == c) s.CurrentSelection = null;
        }
        
        private static CharController[] GetEnemiesInRange(CharStateController s, IEnumerable<CharController> charsInRange) =>
            charsInRange.Where(c => c.TeamID != s.MyCharacter.TeamID).ToArray();
        #endregion
    }
}