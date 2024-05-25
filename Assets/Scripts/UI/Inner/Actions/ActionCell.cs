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
        // Temps
        private int _indexInParent;
        public CharacterAction Action { get; private set; }

        public void SetToAction(CharacterAction action, int index)
        {
            background.gameObject.SetActive(false);
            _indexInParent = index;
            Action = action;
            image.sprite = action.ActionImage;
            nameText.text = $"[{index + 1}]\n{action.ActionName}";
        }
        
        public void SetSelected(bool state) => background.gameObject.SetActive(state);
        public void OnClicked() => CustomInputManager.Instance.ActionCellPressed(_indexInParent, Action);
    }
}