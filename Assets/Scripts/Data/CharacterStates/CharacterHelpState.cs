using UnityEngine;

namespace Data.CharacterStates
{    
    [CreateAssetMenu(fileName = "Help", menuName = "States/Help", order = 10)]
    public class CharacterHelpState : CharacterStateBase
    {
        #region DefaultStateMethods
        protected override void InternalStateSetUp() { }
        public override void StateDisassembly(){ }
        public override bool ExecuteStateFrame() => false;
        #endregion
    }
}