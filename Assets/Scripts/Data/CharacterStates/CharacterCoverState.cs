using Controller;
using UnityEngine;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "Cover", menuName = "States/Cover", order = 10)]
    public class CharacterCoverState : CharacterStateBase
    {
        #region DefaultStateMethods
        
        public override void StateSetUp(CharStateController s) { }
        public override void StateDisassembly(CharStateController s){ }
        public override bool ExecuteStateFrame(CharStateController s) => false;
        
        #endregion
    }
}