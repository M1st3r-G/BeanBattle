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
            // Set state Variables
            s.CurrentSelection = null;
            s.LookingForSelection = false;
            s.AcceptAction.action.Enable();
            
            AudioEffectsManager.Instance.PlayEffect(AudioEffectsManager.AudioEffect.Attack);
            GridManager.Instance.DisplayRange(s.MyCharacter);
            
            // Selects the only enemy in range or enables the player selection if more in range
            CharController[] enemiesInRange = GetEnemiesInRange(s, GridManager.Instance.CharactersInRange());
            if (enemiesInRange.Length == 1) SetSelection(s, enemiesInRange[0]);
            else s.LookingForSelection = true;
        }
        
        public override void StateDisassembly(CharStateController s)
        {
            //Reset State Variables
            s.LookingForSelection = false;
            s.AcceptAction.action.Disable();
            if (s.CurrentSelection is not null) s.CurrentSelection.SetSelector(false);
            s.CurrentSelection = null;
            
            GridManager.Instance.ResetRange();
        }
        
        public override bool ExecuteStateFrame(CharStateController s)
        {
            // Target Selection
            if (s.LookingForSelection)
            {
                if (s.MouseClickAction.WasPerformedThisFrame())
                {
                    if (MouseInputManager.Instance.GetCharacterClicked(out CharController clickedChar))
                    {
                        if (clickedChar.TeamID != s.MyCharacter.TeamID) SetSelection(s, clickedChar);
                    }
                }
            }
            
            if(s.CurrentSelection is null) return false;
            if (!s.AcceptAction.action.WasPerformedThisFrame()) return false;

            // Target is accepted
            s.CurrentSelection.TakeDamage(s.MyCharacter.GetData.Damage);
            return true;
        }
        
        #endregion

        #region StateMethods
        
        /// <summary>
        /// Target the given CharController and marks him
        /// </summary>
        /// <param name="s">The State Controller running the this State</param>
        /// <param name="targetChar">The targeted CharController</param>
        private static void SetSelection(CharStateController s, CharController targetChar)
        {
            if (s.CurrentSelection is not null) s.CurrentSelection.SetSelector(false);

            s.CurrentSelection = targetChar;
            targetChar.SetSelector(true);
        }
        
        private static CharController[] GetEnemiesInRange(CharStateController s, IEnumerable<CharController> charsInRange) =>
            charsInRange.Where(c => c.TeamID != s.MyCharacter.TeamID).ToArray();
        
        #endregion
    }
}