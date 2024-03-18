using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controller
{
    public class CurrentCharacterUIController : MonoBehaviour
    {
        private ActionsUIController _actionController;
        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI nameText; 

        private void Awake()
        {
            _actionController = GetComponentInChildren<ActionsUIController>();
        }

        private void OnChangeEvent(CharController newChar)
        {
            _actionController.SetDisplay(newChar.GetData.Actions);
            portrait.color = newChar.GetData.Material.color;
            nameText.text = newChar.GetData.Name;
        }

        private void OnEnable()
        {
            GameManager.OnCurrentChange += OnChangeEvent;
        }
        
        private void OnDisable()
        {
            GameManager.OnCurrentChange -= OnChangeEvent;
        }
    }
}