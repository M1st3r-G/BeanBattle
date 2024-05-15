using System.Collections.Generic;
using System.Linq;
using Controller;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "Attack", menuName = "States/Attack", order = 10)]
    public class CharacterAttackState : CharacterStateBase
    {
        // ComponentReferences
        [SerializeField] private InputActionReference acceptAction;
        
        // Temps
        private CharController currentSelection;

        #region DefaultStateMethods
        protected override void InternalStateSetUp()
        {
            currentSelection = null;
            acceptAction.action.Enable();
            CharController.OnPlayerDeath += RemoveDeadSelection;
            
            GridManager.Instance.DisplayRange(ActiveCharacter);
            CharController[] enemiesInRange = GetEnemiesInRange(GridManager.Instance.CharactersInRange());

            if (enemiesInRange.Length == 1) SetSelection(enemiesInRange[0]);
            else MouseInputManager.OnCharacterClicked += SelectClickedCharacter;
        }
        
        public override void StateDisassembly()
        {
            acceptAction.action.Disable();
            CharController.OnPlayerDeath -= RemoveDeadSelection;
            MouseInputManager.OnCharacterClicked -= SelectClickedCharacter;
            
            GridManager.Instance.ResetRange();
            
            if (currentSelection is not null) currentSelection.SetSelector(false);
            currentSelection = null;
        }
        
        public override bool ExecuteStateFrame()
        {
            if(currentSelection is null) return false;
            if (!acceptAction.action.WasPerformedThisFrame()) return false;
            
            ActiveCharacter.PerformAttack(currentSelection);
            return true;
        }
        #endregion

        #region StateMethods
        private void SetSelection(CharController c)
        {
            if (currentSelection is not null) currentSelection.SetSelector(false);

            currentSelection = c;
            c.SetSelector(true);
        }
        
        private void SelectClickedCharacter(CharController hitCharacter)
        {
            if (hitCharacter.TeamID != ActiveCharacter.TeamID) SetSelection(hitCharacter);
        }
        
        private void RemoveDeadSelection(CharController c)
        {
            if (currentSelection == c) currentSelection = null;
        }
        
        private CharController[] GetEnemiesInRange(IEnumerable<CharController> charsInRange) =>
            charsInRange.Where(c => c.TeamID != ActiveCharacter.TeamID).ToArray();
        #endregion
    }
}