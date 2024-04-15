using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controller
{
    public class ActionCellController : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI nameText;
        private int _index;
        
        public void SetTo(CharacterActionPacket action, int index)
        {
            _index = index;
            image.sprite = action.ActionImage;
            nameText.text = $"[{index}]\n{action.ActionName}";
        }

        public void OnClicked()
        {
            CurrentCharacterUIController.Instance.ActionCellPressed(_index);
        }
    }
}