using UnityEngine;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "Empty", menuName = "States/Empty", order = 10)]
    public class CharacterEmptyState : CharacterStateBase
    {
        #region DefaultStateMethods
        protected override void InternalStateSetUp() { }
        public override void StateDisassembly(){ }
        public override bool ExecuteStateFrame() => false;
        #endregion
    }
}