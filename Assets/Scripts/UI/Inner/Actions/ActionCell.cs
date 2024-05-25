using Data;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inner.Actions
{
    /// <summary>
    /// Internally used by the <see cref="ActionsUI"/> to store Data, Display Selection and Listen to Clicks (Which are given to the <see cref="CustomInputManager"/>
    /// </summary>
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
        
        public void SetSelectionState(bool state) => background.gameObject.SetActive(state);
        
        /// <summary>
        /// When a Cell is Clicked, its Action is Triggered
        /// </summary>
        public void OnClicked() => CustomInputManager.Instance.ActionCellPressed(_indexInParent, Action);
    }
}