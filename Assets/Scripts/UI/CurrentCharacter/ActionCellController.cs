using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CurrentCharacter
{
    public class ActionCellController : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI nameText;
        private int _index;
        private CharacterAction _action;
        
        public void SetTo(CharacterAction action, int index)
        {
            background.gameObject.SetActive(false);
            _index = index;
            _action = action;
            image.sprite = action.ActionImage;
            nameText.text = $"[{index}]\n{action.ActionName}";
        }

        public void SetSelected(bool state)
        {
            background.gameObject.SetActive(state);
            CurrentActionController.Instance.SetAction(state ? _action : null);
        }
        
        public void OnClicked()
        {
            CurrentCharacterUIController.Instance.ActionCellPressed(_index);
        }
    }
}