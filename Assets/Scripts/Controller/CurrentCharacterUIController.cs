using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Controller
{
    public class CurrentCharacterUIController : MonoBehaviour
    {
        private ActionsUIController _actionController;
        [SerializeField] private Image portrait;

        private void Awake()
        {
            _actionController = GetComponentInChildren<ActionsUIController>();
        }

        private void OnChangeEvent(CharController newChar)
        {
            _actionController.SetDisplay(newChar.GetData.Actions);
            portrait.color = newChar.GetData.Material.color;
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