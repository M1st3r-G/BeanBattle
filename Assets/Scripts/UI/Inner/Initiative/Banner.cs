using Controller;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inner.Initiative
{
    /// <summary>
    /// Internally used by the <see cref="InitiativeUI"/> to store Data, and Display the Characters.
    /// </summary>
    public class Banner : MonoBehaviour
    {
        // Components
        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI initText;
        
        // Temps
        public CharController DisplayedChar { get; private set; }

        public void SetTo(CharController cC)
        {
            CharData data = cC.GetData;
            DisplayedChar = cC;
            
            portrait.color = data.Material(cC.TeamID).color;
            title.text = data.Name;
            healthText.text = cC.CurrentHealth.ToString();
            initText.text = cC.Initiative.ToString();
        }
    }
}