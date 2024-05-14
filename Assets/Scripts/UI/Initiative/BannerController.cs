using Controller;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Initiative
{
    public class BannerController : MonoBehaviour
    {
        // Components
        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI initText;
        public CharController DisplayedChar { get; private set; }

        public void SetTo(CharController cC)
        {
            CharData data = cC.GetData;
            DisplayedChar = cC;
            
            portrait.color = data.Material.color;
            title.text = data.Name;
            healthText.text = cC.CurrentHealth.ToString();
            initText.text = cC.Initiative.ToString();
        }
    }
}