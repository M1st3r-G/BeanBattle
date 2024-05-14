using UnityEngine;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "BeEvil", menuName = "States/BeEvil", order = 10)]
    public class CharacterBeEvilState : CharacterStateBase
    {
        public override bool ExecuteStateFrame() => false;

        protected override void InternalStateSetUp() { }
        public override void StateDisassembly(){ }
    }
}