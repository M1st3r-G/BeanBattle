using System.Collections;
using Controller;
using UnityEngine;

namespace Data.CharacterStates
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
        /* MyVariable = SetUp
         * Play(MySound)
         */
        
        /// <summary>
        /// The State Disassembly should be called everytime the State is exit. It reverses the Variables set in the SetUp.
        /// The State can and should be exit, when no further input is required
        /// </summary>
        /// <param name="s">The StateController running the State, where all Variables are Stored</param>
        /// <seealso cref="StateSetUp"/>
        public abstract void StateDisassembly(CharStateController s);
        /* MyVariable = Deset
         * 
         */
        
        /// <summary>
        /// Executes A frame of the State. It is used for Input Only. A Animation shouled be Carried out by the Animation Coroutine. <see cref="ExecuteStateAndAnimate"/>
        /// </summary>
        /// <param name="s">The StateController running the State, where all Variables are Stored</param>
        /// <returns> True, when the State is over (Input Wise, not Animation wise)</returns>
        public abstract bool ExecuteStateFrame(CharStateController s);
        /* If(Input){ChangeState}
         * If(Accept) {
         * s.MyCharacter.StartCoroutine(ExecuteStateAndAnimate(s));
         * return true}
         */
        
        /// <summary>
        /// The Execution and Animation of the State.
        /// Should be called at the end of <see cref="ExecuteStateFrame"/>.
        /// It should trigger <see cref="Managers.CustomInputManager.DisableInput"/> when it starts.
        /// At the end it should call <see cref="Managers.GameManager.FullActionEnd"/> 
        /// </summary>
        /// <param name="s">The State Controller</param>
        /// <returns>Irrelevant, this is a Coroutine</returns>
        protected abstract IEnumerator ExecuteStateAndAnimate(CharStateController s);
        /* CustomInputManager.DisableInput();
         * Animate();
         * GameManager.FullActionEnd(timeCost -> Currently taken from UI);
         */
    }
}
