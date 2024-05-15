using UnityEngine;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "BeEvil", menuName = "States/BeEvil", order = 10)]
    public class CharacterBeEvilState : CharacterStateBase
    {
        #region DefaultStateMethods
        protected override void InternalStateSetUp() { }
        public override void StateDisassembly(){ }
        public override bool ExecuteStateFrame() => false;
        #endregion
    }
}