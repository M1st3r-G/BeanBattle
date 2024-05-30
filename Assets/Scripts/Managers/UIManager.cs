using Controller;
using Controller.UIControllers;
using Data;
using UIContent;
using UIContent.Actions;
using UIContent.Initiative;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private CurrentCharacterUI currentCharacter;
        [SerializeField] private CurrentActionController currentAction;
        [SerializeField] private InitiativeUI initiativeUI;
        [SerializeField] private ActionsUI action;
        [SerializeField] private GameOverController gameOver;
        
        private int CurrentSelection { get; set; } = -1;
        
        public static UIManager Instance { get; private set; }

        #endregion

        #region SetUp

        private void Awake()
        {
            Instance = this;
            gameOver.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            GameManager.OnGameOver += OnGameOver;
            CharController.OnPlayerDeath += OnPlayerDeath;
            CharController.OnPlayerFinishedAction += DeselectCurrentActionWrapper;
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= OnGameOver;
            CharController.OnPlayerDeath -= OnPlayerDeath;
            CharController.OnPlayerFinishedAction -= DeselectCurrentActionWrapper;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        #endregion

        #region EventMethods

        private void OnPlayerDeath(CharController c)
        {
            initiativeUI.RemoveDeadPlayer(c);
        }

        private void OnGameOver(int winningTeam)
        {
            // Hides all UI
            currentCharacter.gameObject.SetActive(false); 
            currentAction.gameObject.SetActive(false);
            initiativeUI.gameObject.SetActive(false);
            action.gameObject.SetActive(false);
                
            // Shows Game Over
            gameOver.gameObject.SetActive(true);
            gameOver.OnGameOverEvent(winningTeam);
        }

        #endregion

        #region CharacterDisplay

        public void ChangeInitiativeOrderTo(CharController[] newOrder) => initiativeUI.UpdateUI(newOrder);
        
        public void ChangeActiveCharacter(CharController newChar)
        {
            currentCharacter.SetCharacter(newChar);
            action.SetDisplay(newChar.GetData.Actions);
        }
        
        public void RefreshCharacter(CharController c) => initiativeUI.RefreshCharacter(c);
        

        #endregion
        
        #region ActionSelection

        /// <summary>
        /// Triggered by the <see cref="CustomInputManager"/> Visually Selects the Action at the given Index. It refers to the <see cref="ActionsUI._actionCells"/> Order
        /// </summary>
        /// <param name="actionIndex">The (Zero Based) Index of the Action</param>
        /// <seealso cref="ActionsUI"/>
        public void SelectAction(int actionIndex)
        {
            // When the Selected Action is Selected Again, it should be only deselected instead   
            if (actionIndex == CurrentSelection) DeselectCurrentAction();
            else DisplayNewAction(actionIndex);
        }

        /// <summary>
        /// Internally used to set the <see cref="ActionsUI"/> Cell at the Index Active and Adjust the <see cref="CurrentActionController"/>
        /// </summary>
        /// <param name="actionIndex">The (Zero Based) Index of the Action Cell</param>
        /// <seealso cref="ActionsUI"/>
        private void DisplayNewAction(int actionIndex)
        {
            // Disable old ActionCell
            if (CurrentSelection != -1) action.SetCellStateAtIndex(CurrentSelection, false, out _);

            // Select new Action
            CurrentSelection = actionIndex;
            action.SetCellStateAtIndex(actionIndex, true, out CharacterAction actionInCell);
            currentAction.SetAction(actionInCell);

            Debug.Log($"Displayed the {actionInCell}Action Visuals");
        }

        /// <summary>
        /// Deselect the current Action in the <see cref="CurrentActionController"/> and the <see cref="ActionCell"/> in the <see cref="ActionsUI"/>
        /// </summary>
        private void DeselectCurrentAction()
        {
            action.SetCellStateAtIndex(CurrentSelection, false, out _);
            currentAction.SetAction(null);
            CurrentSelection = -1;
                
            Debug.Log("Hid the Current Action Visuals");
        }
        
        /// <summary>
        /// Wrapper to Disable the Current Action when a Player Action end, i.e. the <see cref="CharStateController"/> full ends with a <see cref="CharController.OnPlayerFinishedAction"/> Event
        /// </summary>
        /// <param name="disabledAction">The Action that was Disabled</param>
        private void DeselectCurrentActionWrapper(CharacterAction.ActionTypes disabledAction)
        {
            // If nothing is Selected Ignore
            if (CurrentSelection == -1) return;
            
            // Deselect Action when Finished, i.e. only if the Type is the Current type 
            if (disabledAction == action.GetActionWithIndex(CurrentSelection).Type) 
                DeselectCurrentAction();
        }
        
        #endregion

        #region GetAndSet

        public CharacterAction GetActionWithIndex(int index) => action.GetActionWithIndex(index);
        public void SetTimeCost(int timeCost) => currentAction.SetTimeCost(timeCost);
        public int GetTimeCost() => currentAction.GetTimeCost();

        #endregion
        
    }
}