using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GameOverController : MonoBehaviour
    {
        private CanvasGroup _cg;
        
        [SerializeField] private TextMeshProUGUI teamText;
        [SerializeField] private TextMeshProUGUI turnsText;
        [SerializeField] private TextMeshProUGUI stepsText;
        private void Awake()
        {
            _cg = GetComponent<CanvasGroup>();
            _cg.alpha = 0f;
            _cg.blocksRaycasts = _cg.interactable = false;
        }

        private void OnEnable()
        {
            GameManager.OnGameOver += OnGameOverEvent;
        }

        private void OnGameOverEvent(int winningTeam)
        {
            _cg.alpha = 1f;
            _cg.blocksRaycasts = _cg.interactable = true;

            teamText.text = $"Team {winningTeam} is the winning Team!";
            
            Tuple<int, int> stats = GameManager.Instance.GetGameStats();
            turnsText.text = $"The Game Lasted a total of {stats.Item1} Turns";
            stepsText.text = $"The Characters took a total of {stats.Item2} Steps";
        }

        public void MenuButtonAction() => SceneManager.LoadScene(0);
        public void ResetButtonAction() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}