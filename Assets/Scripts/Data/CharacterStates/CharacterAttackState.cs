using UnityEngine;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "Attack", menuName = "States/Attack", order = 10)]
    public class CharacterAttackState : CharacterStateBase
    {
        public override bool ExecuteStateFrame() => false;
        public override void InternalStateSetUp() { }
        public override void StateDisassembly(){ }
    }
}