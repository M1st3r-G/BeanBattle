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
        
        private void Awake()
        {
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