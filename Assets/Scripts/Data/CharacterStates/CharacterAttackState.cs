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
            if (currentSelection is null)
            {
                return false;
            }
            else
            {
                Debug.Log("Wait For Accept"); 
            }
            return false;
        }

        public override void InternalStateSetUp()
        {
            currentSelection = null;
            attackRange = ActiveCharacter.GetData.AttackRange;
            GridManager.Instance.DisplayRange(ActiveCharacter, attackRange);

            int numOfChars = GridManager.Instance.CharactersInRange(out CharController[] charsInRange);
            if (numOfChars == 1)
            {
                Debug.LogError("There is no one in Range");
            }
            if(numOfChars == 2){
                Debug.LogWarning("Found only two Characters");
                //SetSelection(charsInRange);
            }
            else
            {
                MouseInputManager.OnCharacterClicked += Stuff;
            }
        }

        private void SetSelection(CharController c)
        {
            if (currentSelection is not null) c.SetSelector(false);

            currentSelection = c;
            c.SetSelector(true);
        }
        
        private void Stuff(CharController hitCharacter)
        {
            if (hitCharacter != ActiveCharacter)
            {
                Debug.LogWarning($"Target {hitCharacter.gameObject.name}");
                SetSelection(hitCharacter);
            }

            Debug.LogWarning("Target Self");
        }

        public override void StateDisassembly()
        {
            if (currentSelection is not null) currentSelection.SetSelector(false);
            GridManager.Instance.ResetRange();
            MouseInputManager.OnCharacterClicked -= Stuff;
        }
    }
}