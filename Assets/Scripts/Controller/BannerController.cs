using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controller
{
    public class BannerController : MonoBehaviour
    {
        // Components
        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI initText;
        
        public void SetTo(CharController cC)
        {
            CharData data = cC.GetData;

            portrait.color = data.Material.color;
            title.text = cC.GetData.Name;
            healthText.text = cC.CurrentHealth.ToString();
            initText.text = cC.Initiative.ToString();
        }
    }
}