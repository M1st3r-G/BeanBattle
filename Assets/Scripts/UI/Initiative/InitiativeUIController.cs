using Controller;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Initiative
{
    public class InitiativeUIController : MonoBehaviour
    {
        // ComponentReferences
        private BannerController[] _banner = new BannerController[12];
        private VerticalLayoutGroup _layoutGroup;

        // Public
        public static InitiativeUIController Instance { get; private set; }

        #region SetUp
        private void Awake()
        {
            if (Instance is not null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            _layoutGroup = GetComponent<VerticalLayoutGroup>();
            _banner = GetComponentsInChildren<BannerController>(true);
        }
        
        private void OnEnable()
        {
            GameManager.OnOrderChanged += UpdateUI;
            CharController.OnPlayerDeath += RemoveDeadPlayer;
            GameManager.OnGameOver += OnGameOver;
        }
        
        private void OnDisable()
        {
            GameManager.OnOrderChanged -= UpdateUI;
            CharController.OnPlayerDeath -= RemoveDeadPlayer;
            GameManager.OnGameOver -= OnGameOver;
        }
        #endregion

        #region MainMethods
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
        
        public void RefreshCharacter(CharController c)
        {
            foreach (BannerController banner in _banner) if (banner.DisplayedChar == c) banner.SetTo(c);
        }
        
        private void OnGameOver(int _) => gameObject.SetActive(false);
        private void AdjustBottomPadding(int numberOfActives) => _layoutGroup.padding.bottom = Mathf.Max(15, -150 * numberOfActives + 1065);
        #endregion
    }
}