using Controller;
using Managers;
using UnityEngine;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "Attack", menuName = "States/Attack", order = 10)]
    public class CharacterAttackState : CharacterStateBase
    {
        private int attackRange;
        private CharController currentSelection;
        
        public override bool ExecuteStateFrame()
        {
            return false;
        }

        public override void InternalStateSetUp()
        {
            currentSelection = null;
            attackRange = ActiveCharacter.GetData.AttackRange;
            GridManager.Instance.DisplayRange(ActiveCharacter, attackRange);

            CharController[] charsInRange = GridManager.Instance.CharactersInRange();
            //Filter Characters of opposing teams
            switch (charsInRange.Length)
            {
                case 1:
                    Debug.LogError("There is no one in Range");
                    break;
                case 2:
                    SetSelection(charsInRange[0] == ActiveCharacter ? charsInRange[1] : charsInRange[0]);
                    break;
                default:
                    MouseInputManager.OnCharacterClicked += SelectClickedCharacter;
                    break;
            }
        }

        private void SetSelection(CharController c)
        {
            if (currentSelection is not null) currentSelection.SetSelector(false);

            currentSelection = c;
            c.SetSelector(true);
        }
        
        private void SelectClickedCharacter(CharController hitCharacter)
        {
            if (hitCharacter == ActiveCharacter)
            {
                Debug.LogWarning("Target Self");
            }
            else
            {
                Debug.LogWarning($"Target {hitCharacter.gameObject.name}");
                SetSelection(hitCharacter);
            }
        }

        public override void StateDisassembly()
        {
            if (currentSelection is not null) currentSelection.SetSelector(false);
            GridManager.Instance.ResetRange();
            MouseInputManager.OnCharacterClicked -= SelectClickedCharacter;
        }
    }
}