using UnityEngine;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "Empty", menuName = "States/Empty", order = 10)]
    public class CharacterEmptyState : CharacterStateBase
    {
        public override bool ExecuteStateFrame() => false;
        public override void InternalStateSetUp() { }
        public override void StateDisassembly(){ }
    }
}