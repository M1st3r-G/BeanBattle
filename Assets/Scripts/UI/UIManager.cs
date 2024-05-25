using Controller;
using Data;
using Managers;
using UI.Inner;
using UI.Inner.Actions;
using UI.Inner.Initiative;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private CurrentCharacterUI currentCharacter;
        [SerializeField] private CurrentActionController currentAction;
        [SerializeField] private InitiativeUI initiativeUI;
        [SerializeField] private ActionsUI action;

        private int CurrentSelection { get; set; } = -1;
        
        public static UIManager Instance { get; private set; }

        #region SetUp

        private void Awake()
        {
            if (Instance is not null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
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

        #endregion

        #region EventMethods

        private void OnPlayerDeath(CharController c)
        {
            initiativeUI.RemoveDeadPlayer(c);
        }

        private void OnGameOver(int winningTeam)
        {
            DeselectCurrentAction();
            gameObject.SetActive(false);
        }

        #endregion
        
        public void ChangeInitiativeOrderTo(CharController[] newOrder) => initiativeUI.UpdateUI(newOrder);
        
        public void ChangeActiveCharacter(CharController newChar)
        {
            currentCharacter.SetCharacter(newChar);
            action.SetDisplay(newChar.GetData.Actions);
        }
        
        public void RefreshCharacter(CharController c) => initiativeUI.RefreshCharacter(c);

        #region ActionSelection

        public void SelectAction(int actionIndex)
        {
            if (actionIndex == CurrentSelection)
            {
                DeselectCurrentAction();
                currentAction.SetAction(null);
            }
            else
            {
                CurrentSelection = actionIndex;
                action.SetCellAtIndex(actionIndex, true, out CharacterAction actionInCell);
                currentAction.SetAction(actionInCell);
            }
        }

        private void DeselectCurrentAction() => action.SetCellAtIndex(CurrentSelection, false, out _);

        #endregion

        #region GetAndSet

        public CharacterAction GetActionWithIndex(int index) => action.GetActionWithIndex(index);
        public void SetTimeCost(int timeCost) => currentAction.SetTimeCost(timeCost);
        public int GetTimeCost() => currentAction.GetTimeCost();

        #endregion
        
    }
}