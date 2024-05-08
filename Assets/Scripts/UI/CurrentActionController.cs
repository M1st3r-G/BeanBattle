using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CurrentActionController : MonoBehaviour
    {
        //ComponentReferences
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI timeValue;
        //Params
        //Temps
        //Public
        public static CurrentActionController Instance { get; private set; }
     
        private void Awake()
        {
            if (Instance is not null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

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

        public void SetTimeCost(int value)
        {
            timeValue.text = value.ToString();
        }
    }
}