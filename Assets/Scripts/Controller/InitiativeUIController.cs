using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Controller
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

        private void UpdateUI(List<CharController> currentOrder)
        {
            for (int i = 0; i < currentOrder.Count; i++)
            {
                _banner[i].gameObject.SetActive(true);
                _banner[i].SetTo(currentOrder[i]);
            }

            for (int i = currentOrder.Count; i < 12; i++)
            {
                _banner[i].gameObject.SetActive(false);
            }

            AdjustBottomPadding(currentOrder.Count);
        }

        private void AdjustBottomPadding(int numberOfActives)
        {
            _layoutGroup.padding.bottom = Mathf.Max(15, -150 * numberOfActives + 1065);
        }
    }
}