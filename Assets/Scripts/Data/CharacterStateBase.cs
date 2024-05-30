using Controller;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// This Asset Saves the Code for a State manageable by the <see cref="CharStateController"/> of a <see cref="CharController"/>
    /// </summary>
    public abstract class CharacterStateBase: ScriptableObject
    {
        public CharacterAction.ActionTypes ActionType => currentType;
        [SerializeField] private CharacterAction.ActionTypes currentType;
        
        /// <summary>
        /// The State SetUp should be called Once before any Frame Update. It should set State Variables and play Inital Sounds. 
        /// </summary>
        /// <param name="s">The StateController running the State, where all Variables are Stored</param>
        /// <seealso cref="StateDisassembly"/>
        public abstract void StateSetUp(CharStateController s);
        
        /// <summary>
        /// The State Disassembly should be called everytime the State is changed or exit. It reverses the Variables set in the SetUp
        /// A True end (With animation) should manually be triggered with by setting the <see cref="CharStateController.IsAnimating"/> bool during Animation and relasing it when finished. Also Call a <see cref="CharController.OnPlayerFinishedAction"/> Event
        /// </summary>
        /// <param name="s">The StateController running the State, where all Variables are Stored</param>
        /// <seealso cref="StateSetUp"/>
        public abstract void StateDisassembly(CharStateController s);
        
        /// <summary>
        /// Executes A frame of the State. It is used for Input Only. A Animation shouled be Carried out by the Animation Coroutine.
        /// Which should setting the <see cref="CharStateController.IsAnimating"/> bool during Animation and call a <see cref="CharController.OnPlayerStartedAction"/> Event.
        /// When the Animation ends, reset the bool and call a <see cref="CharController.OnPlayerFinishedAction"/> Event.
        /// </summary>
        /// <param name="s">The StateController running the State, where all Variables are Stored</param>
        /// <returns> True, when the State is over (Input Wise, not Animation wise)</returns>
        public abstract bool ExecuteStateFrame(CharStateController s);
    }
}
