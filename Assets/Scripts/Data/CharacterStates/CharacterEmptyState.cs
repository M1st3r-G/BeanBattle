using Controller;
using UnityEngine;

namespace Data.CharacterStates
{
    [CreateAssetMenu(fileName = "Empty", menuName = "States/Empty", order = 10)]
    public class CharacterEmptyState : CharacterStateBase
    {
        #region DefaultStateMethods
        public override void StateSetUp(CharStateController s) { }
        public override void StateDisassembly(CharStateController s){ }
        public override bool ExecuteStateFrame(CharStateController s) => false;
        public override void OnPlayerDeath(CharStateController s, CharController deadPlayer) { }
        #endregion
    }
}