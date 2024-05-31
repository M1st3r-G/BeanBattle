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
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= OnGameOver;
            CharController.OnPlayerDeath -= OnPlayerDeath;
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
        /// Used to set the <see cref="ActionsUI"/> Cell at the Index Active and Adjust the <see cref="CurrentActionController"/>
        /// </summary>
        /// <param name="newActionType">The Action to Display</param>
        /// <seealso cref="ActionsUI"/>
        public void DisplayNewAction(CharacterAction.ActionTypes newActionType)
        {
            // Disable old ActionCell
            if (CurrentSelection != -1) action.SetCellStateAtIndex(CurrentSelection, false);

            int index = action.GetIndexWithType(newActionType, out var actionAsset);    
            
            // Select new Action
            CurrentSelection = index;
            action.SetCellStateAtIndex(index, true);
            currentAction.SetAction(actionAsset);

            Debug.Log($"Displayed the {newActionType}Action Visuals");
        }

        /// <summary>
        /// Deselect the current Action in the <see cref="CurrentActionController"/> and the <see cref="ActionCell"/> in the <see cref="ActionsUI"/>
        /// </summary>
        public void DeselectCurrentAction()
        {
            action.SetCellStateAtIndex(CurrentSelection, false);
            currentAction.SetAction(null);
            CurrentSelection = -1;
                
            Debug.Log("Hid the Current Action Visuals");
        }
        
        #endregion

        #region GetAndSet

        public CharacterAction GetActionWithIndex(int index) => action.GetActionWithIndex(index);
        public void SetTimeCost(int timeCost) => currentAction.SetTimeCost(timeCost);
        public int GetTimeCost() => currentAction.GetTimeCost();

        #endregion
        
    }
}