using Managers;
using UnityEngine;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "Attack", menuName = "States/Attack", order = 10)]
    public class CharacterAttackState : CharacterStateBase
    {
        private int attackRange;
        public override bool ExecuteStateFrame() => false;

        public override void InternalStateSetUp()
        {
            attackRange = ActiveCharacter.GetData.AttackRange;
            GridManager.Instance.DisplayRange(ActiveCharacter, attackRange);
        }

        public override void StateDisassembly()
        {
            GridManager.Instance.ResetRange();
        }
    }
}