using Data;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inner.Actions
{
    public class ActionCell : MonoBehaviour
    {
        // ComponentReferences
        [SerializeField] private Image background;
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI nameText;
        private CurrentActionController _parentActionController;
        
        // Temps
        private int _parentIndex;

        public CharacterAction Action { get; private set; }

        private void Awake()
        {
            _parentActionController = GetComponentInParent<CurrentActionController>();
        }

        public void SetTo(CharacterAction action, int index)
        {
            background.gameObject.SetActive(false);
            _parentIndex = index;
            Action = action;
            image.sprite = action.ActionImage;
            nameText.text = $"[{index}]\n{action.ActionName}";
        }

        public void SetSelected(bool state)
        {
            background.gameObject.SetActive(state);
            _parentActionController.SetAction(state ? Action : null);
        }
        
        public void OnClicked() => CustomInputManager.Instance.ActionCellPressed(_parentIndex, Action);
    }
}