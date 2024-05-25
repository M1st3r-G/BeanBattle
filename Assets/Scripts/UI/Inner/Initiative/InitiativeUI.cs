using Controller;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inner.Initiative
{
    /// <summary>
    /// Internally used by the <see cref="UIManager"/>. Uses <see cref="Banner"/> internally to Display and Manage the Characters
    /// </summary>
    public class InitiativeUI : MonoBehaviour
    {
        // ComponentReferences
        private Banner[] _banner = new Banner[12];
        private VerticalLayoutGroup _layoutGroup;

        private void Awake()
        {
            _layoutGroup = GetComponent<VerticalLayoutGroup>();
            _banner = GetComponentsInChildren<Banner>(true);
        }

        /// <summary>
        /// Displays the Characters int the Array in the Given Order
        /// </summary>
        /// <param name="currentOrder">The Character in Order to be Displayed</param>
        public void UpdateUI(CharController[] currentOrder)
        {
            // Display the Characters
            for (int i = 0; i < currentOrder.Length; i++)
            {
                _banner[i].gameObject.SetActive(true);
                _banner[i].SetTo(currentOrder[i]);
            }

            // Hide the others
            for (int i = currentOrder.Length; i < 12; i++)
            {
                _banner[i].gameObject.SetActive(false);
            }

            // Adjust Spacing
            AdjustBottomPadding(currentOrder.Length);
        }

        /// <summary>
        /// Removes a Dead player from the List (Before the Default Update)
        /// </summary>
        /// <param name="player">The Player to be removed</param>
        public void RemoveDeadPlayer(CharController player)
        {
            // Also Counts how many of the characters are still active to adjust spacing
            int counter = 0;
            foreach (Banner banner in _banner)
            {
                if (banner.DisplayedChar == player) banner.gameObject.SetActive(false);
                if (banner.gameObject.activeSelf) counter++;
            }
            
            AdjustBottomPadding(counter - 1);
        }
        
        /// <summary>
        /// Re-Displays the Information of the <see cref="Banner"/> containing the Character
        /// </summary>
        /// <param name="c">The Character whose Information were Changed</param>
        public void RefreshCharacter(CharController c)
        {
            foreach (Banner banner in _banner) if (banner.DisplayedChar == c) banner.SetTo(c);
        }
        
        private void AdjustBottomPadding(int numberOfActives) => _layoutGroup.padding.bottom = Mathf.Max(15, -150 * numberOfActives + 1065);
    }
}