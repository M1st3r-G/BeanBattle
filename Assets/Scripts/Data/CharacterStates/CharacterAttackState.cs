using System.Collections;
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
            
            AudioEffectsManager.Instance.PlayEffect(AudioEffectsManager.AudioEffect.Attack);
            
            GridManager.Instance.ResetRange();
            GridManager.Instance.DisplayRange(s.MyCharacter);
            
            // Selects the only enemy in range or enables the player selection if more in range
            CharController[] enemiesInRange = GetEnemiesInRange(s, GridManager.Instance.CharactersInRange());
            if (enemiesInRange.Length == 1) SetSelection(s, enemiesInRange[0]);
            else s.LookingForSelection = true;
        }
        
        public override void StateDisassembly(CharStateController s)
        {
            //Reset State Variables
            if (s.CurrentSelection is not null) s.CurrentSelection.SetSelector(false);
            GridManager.Instance.ResetRange();
        }
        
        public override bool ExecuteStateFrame(CharStateController s)
        {
            // Target Selection
            if (s.LookingForSelection && CustomInputManager.Instance.MouseClickedThisFrame())
            {
                Debug.Log("AttackState Noticed Click");
                if (MouseInputManager.Instance.GetCharacterClicked(out CharController clickedChar))
                {
                    if (clickedChar.TeamID != s.MyCharacter.TeamID) SetSelection(s, clickedChar);
                    Debug.LogError($"Clicked Character{clickedChar.name}");
                }
            }
            
            if(s.CurrentSelection is null) return false;
            if (!CustomInputManager.Instance.AcceptedThisFrame()) return false;
            // Target is accepted

            s.MyCharacter.StartCoroutine(ExecuteStateAndAnimate(s));
            return true;
        }

        protected override IEnumerator ExecuteStateAndAnimate(CharStateController s)
        {
            CustomInputManager.Instance.DisableInput();
            yield return new WaitForSeconds(s.MyCharacter.Attack(s.CurrentSelection.transform));
            s.CurrentSelection.TakeDamage(s.MyCharacter.GetData.Damage);
            yield return null;
            GameManager.Instance.FullActionEnd();
        }

        #endregion

        #region StateMethods
        
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