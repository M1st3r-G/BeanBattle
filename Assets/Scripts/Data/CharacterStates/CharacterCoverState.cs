using UnityEngine;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "Cover", menuName = "States/Cover", order = 10)]
    public class CharacterCoverState : CharacterStateBase
    {
        public override bool ExecuteStateFrame() => false;

        protected override void InternalStateSetUp() { }
        public override void StateDisassembly(){ }
    }
}