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
            GameManager.OnCurrentChange += OnActiveCharacterChangeEvent;
            GameManager.OnGameOver += OnGameOver;
            CharController.OnPlayerDeath += OnPlayerDeath;
            GameManager.OnOrderChanged += UpdateUI;

        }
        
        private void OnDisable()
        {
            GameManager.OnCurrentChange -= OnActiveCharacterChangeEvent;
            GameManager.OnGameOver -= OnGameOver;
            CharController.OnPlayerDeath -= OnPlayerDeath;
            GameManager.OnOrderChanged -= UpdateUI;
        }

        private void UpdateUI(CharController[] currentOrder)
        {
            initiativeUI.UpdateUI(currentOrder);
        }
        
        private void OnPlayerDeath(CharController c)
        {
            initiativeUI.RemoveDeadPlayer(c);
        }

        private void OnGameOver(int winningTeam)
        {
            DeselectCurrentAction();
            gameObject.SetActive(false);
        }
        
        private void OnActiveCharacterChangeEvent(CharController newChar)
        {
            currentCharacter.SetCharacter(newChar);
            action.SetDisplay(newChar.GetData.Actions);
        }
        
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

        public void RefreshCharacter(CharController c)
        {
            initiativeUI.RefreshCharacter(c);
        }

        public void SetTimeCost(int timeCost)
        {
            currentAction.SetTimeCost(timeCost);
        }

        public CharacterAction GetActionWithIndex(int index) => action.GetActionWithIndex(index);
        
        public int GetTimeCost() => currentAction.GetTimeCost();
        
        private void DeselectCurrentAction() => action.SetCellAtIndex(CurrentSelection, false, out _);
    }
}