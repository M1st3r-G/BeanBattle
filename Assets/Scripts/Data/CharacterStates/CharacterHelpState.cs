using UnityEngine;

namespace Data.CharacterStates
{    
    [CreateAssetMenu(fileName = "Help", menuName = "States/Help", order = 10)]
    public class CharacterHelpState : CharacterStateBase
    {
        public override bool ExecuteStateFrame() => false;
        public override void InternalStateSetUp() { }
        public override void StateDisassembly(){ }
    }
}