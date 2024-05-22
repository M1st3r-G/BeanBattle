using Controller;
using UnityEngine;

namespace Data
{
    public abstract class CharacterStateBase: ScriptableObject
    {
        public CharacterAction.ActionTypes Type => type;
        [SerializeField] private CharacterAction.ActionTypes type;

        public abstract void OnPlayerDeath(CharStateController s, CharController deadPlayer);
        public abstract void StateSetUp(CharStateController s);
        public abstract void StateDisassembly(CharStateController s);
        public abstract bool ExecuteStateFrame(CharStateController s);
    }
}
