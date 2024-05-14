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

        public void UpdateHealth()
        {
            foreach (BannerController banner in _banner)
            {
                banner.UpdateHealth();
            }
        }
        
        private void AdjustBottomPadding(int numberOfActives)
        {
            _layoutGroup.padding.bottom = Mathf.Max(15, -150 * numberOfActives + 1065);
        }

        public void SetInitiative(CharController c)
        {
            _banner[0].SetTo(c);
        }
        
        private void OnEnable()
        {
            GameManager.OnOrderChanged += UpdateUI;
        }

        private void OnDisable()
        {
            GameManager.OnOrderChanged -= UpdateUI;
        }
    }
}