using Controller;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inner.Initiative
{
    public class InitiativeUI : MonoBehaviour
    {
        // ComponentReferences
        private Banner[] _banner = new Banner[12];
        private VerticalLayoutGroup _layoutGroup;

        // Public

        private void Awake()
        {
            _layoutGroup = GetComponent<VerticalLayoutGroup>();
            _banner = GetComponentsInChildren<Banner>(true);
        }

        #region MainMethods
        public void UpdateUI(CharController[] currentOrder)
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

        public void RemoveDeadPlayer(CharController player)
        {
            int counter = 0;
            foreach (Banner banner in _banner)
            {
                if (banner.DisplayedChar == player) banner.gameObject.SetActive(false);
                if (banner.gameObject.activeSelf) counter++;
            }
            
            AdjustBottomPadding(counter - 1);
        }
        
        public void RefreshCharacter(CharController c)
        {
            foreach (Banner banner in _banner) if (banner.DisplayedChar == c) banner.SetTo(c);
        }
        
        private void AdjustBottomPadding(int numberOfActives) => _layoutGroup.padding.bottom = Mathf.Max(15, -150 * numberOfActives + 1065);
        #endregion
    }
}