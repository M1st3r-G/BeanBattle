using Controller;
using UnityEngine;

namespace Data
{
    public abstract class CharacterStateBase: ScriptableObject
    {
        protected CharController ActiveCharacter;
        
        public CharacterAction.ActionTypes Type => type;
        [SerializeField] private CharacterAction.ActionTypes type;
        
        public void StateSetUp(CharController pActiveCharacter)
        {
            ActiveCharacter = pActiveCharacter;
            InternalStateSetUp();   
        }
        
        public abstract bool ExecuteStateFrame();
        protected abstract void InternalStateSetUp();
        public abstract void StateDisassembly();
    }
}
