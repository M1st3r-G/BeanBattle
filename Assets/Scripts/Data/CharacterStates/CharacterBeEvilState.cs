using Controller;
using UnityEngine;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "BeEvil", menuName = "States/BeEvil", order = 10)]
    public class CharacterBeEvilState : CharacterStateBase
    {
        #region DefaultStateMethods

        public override void OnPlayerDeath(CharStateController s, CharController deadPlayer) { }
        public override void StateSetUp(CharStateController s) { }
        public override void StateDisassembly(CharStateController s){ }
        public override bool ExecuteStateFrame(CharStateController s) => false;
        #endregion
    }
}