using Data;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIContent
{
    /// <summary>
    /// Internally used by the <see cref="UIManager"/> to Display the Current Action
    /// </summary>
    public class CurrentActionController : MonoBehaviour
    {
        //ComponentReferences
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI timeValue;

        private void Awake() => gameObject.SetActive(false);

        public void SetAction(CharacterAction action)
        {
            if (action is null)
            {
                gameObject.SetActive(false);
                return;
            }
            
            gameObject.SetActive(true);
            icon.sprite = action.ActionImage;
            title.text = action.ActionName;
            description.text = action.ActionDescription;
            SetTimeCost(3);
        }

        public int GetTimeCost() => int.Parse(timeValue.text);
        public void SetTimeCost(int value) => timeValue.text = value.ToString();
    }
}