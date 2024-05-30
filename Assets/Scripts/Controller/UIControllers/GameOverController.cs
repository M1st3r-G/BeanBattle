using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller.UIControllers
{
    public class GameOverController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI teamText;
        [SerializeField] private TextMeshProUGUI turnsText;
        [SerializeField] private TextMeshProUGUI stepsText;

        private void OnEnable()
        {
            GameManager.OnGameOver += OnGameOverEvent;
        }

        
        /// <summary>
        /// Refreshes the Text. The Display is Carried by the <see cref="UIManager"/>
        /// </summary>
        /// <param name="winningTeam"></param>
        private void OnGameOverEvent(int winningTeam)
        {
            teamText.text = $"Team {winningTeam} is the winning Team!";
            
            Tuple<int, int> stats = GameManager.Instance.GetGameStats();
            turnsText.text = $"The Game Lasted a total of {stats.Item1} Turns";
            stepsText.text = $"The Characters took a total of {stats.Item2} Steps";
        }

        public void MenuButtonAction() => SceneManager.LoadScene(0);
        public void ResetButtonAction() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}