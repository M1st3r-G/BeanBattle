using Controller;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Initiative
{
    public class InitiativeUIController : MonoBehaviour
    {
        private BannerController[] _banner = new BannerController[12];
        private VerticalLayoutGroup _layoutGroup;

        public static InitiativeUIController Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this; // TODO Dangerous
            
            _layoutGroup = GetComponent<VerticalLayoutGroup>();
            _banner = GetComponentsInChildren<BannerController>(true);
        }

        private void UpdateUI(CharController[] currentOrder)
        {
            for (int i = 0; i < currentOrder.Length; i++)
            {
                _banner[i].gameObject.SetActive(true);
                _banner[i].SetTo(currentOrder[i]);
            }

            for (int i = currentOrder.Length; i < 12; i++)
            {
                _banner[i].gameObject.SetActive(false);
            }

            AdjustBottomPadding(currentOrder.Length);
        }

        private void AdjustBottomPadding(int numberOfActives)
        {
            _layoutGroup.padding.bottom = Mathf.Max(15, -150 * numberOfActives + 1065);
        }

        public void RefreshCharacter(CharController c)
        {
            foreach (BannerController banner in _banner)
            {
                if (banner.DisplayedChar == c)
                {
                    banner.SetTo(c);
                }
            }
        }

        private void RemoveDeadPlayer(CharController player)
        {
            int counter = 0;
            foreach (BannerController banner in _banner)
            {
                if (banner.DisplayedChar == player) banner.gameObject.SetActive(false);
                if (banner.gameObject.activeSelf) counter++;
            }
            
            AdjustBottomPadding(counter - 1);
        }
        
        private void OnEnable()
        {
            GameManager.OnOrderChanged += UpdateUI;
            CharController.OnPlayerDeath += RemoveDeadPlayer;
            GameManager.OnGameOver += OnGameOver;
        }

        private void OnGameOver(int _)
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            GameManager.OnOrderChanged -= UpdateUI;
            CharController.OnPlayerDeath -= RemoveDeadPlayer;
            GameManager.OnGameOver -= OnGameOver;
        }
    }
}