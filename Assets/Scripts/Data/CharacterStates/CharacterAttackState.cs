using Controller;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "Attack", menuName = "States/Attack", order = 10)]
    public class CharacterAttackState : CharacterStateBase
    {
        [SerializeField] private InputActionReference acceptAction;
        private int attackRange;
        private CharController currentSelection;
        
        public override bool ExecuteStateFrame()
        {
            if(currentSelection is null) return false;
            if (!acceptAction.action.WasPerformedThisFrame()) return false;
            
            ActiveCharacter.PerformAttack(currentSelection);
            return true;
        }

        protected override void InternalStateSetUp()
        {
            currentSelection = null;
            attackRange = ActiveCharacter.GetData.AttackRange;
            GridManager.Instance.DisplayRange(ActiveCharacter, attackRange);

            acceptAction.action.Enable();
            
            CharController[] charsInRange = GridManager.Instance.CharactersInRange();
            //Filter Characters of opposing teams
            switch (charsInRange.Length)
            {
                case 1:
                    break;
                case 2:
                    SetSelection(charsInRange[0] == ActiveCharacter ? charsInRange[1] : charsInRange[0]);
                    break;
                default:
                    MouseInputManager.OnCharacterClicked += SelectClickedCharacter;
                    break;
            }

            CharController.OnPlayerDeath += RemoveDeadSelection;
        }

        private void SetSelection(CharController c)
        {
            if (currentSelection is not null) currentSelection.SetSelector(false);

            currentSelection = c;
            c.SetSelector(true);
        }
        
        private void SelectClickedCharacter(CharController hitCharacter)
        {
            if (hitCharacter != ActiveCharacter) SetSelection(hitCharacter);
        }

        private void RemoveDeadSelection(CharController c)
        {
            if (currentSelection == c) currentSelection = null;
        }
        
        public override void StateDisassembly()
        {
            CharController.OnPlayerDeath -= RemoveDeadSelection;
            acceptAction.action.Disable();
            
            if (currentSelection is not null) currentSelection.SetSelector(false);
            currentSelection = null;
            
            GridManager.Instance.ResetRange();
            MouseInputManager.OnCharacterClicked -= SelectClickedCharacter;
        }
    }
}