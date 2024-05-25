using Controller;
using UnityEngine;

namespace Data.CharacterStates
{    
    [CreateAssetMenu(fileName = "Help", menuName = "States/Help", order = 10)]
    public class CharacterHelpState : CharacterStateBase
    {
        #region DefaultStateMethods
        
        public override void StateSetUp(CharStateController s) { }
        public override void StateDisassembly(CharStateController s){ }
        public override bool ExecuteStateFrame(CharStateController s) => false;
        
        #endregion
    }
}