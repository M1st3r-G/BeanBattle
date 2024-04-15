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

        public void SetTo(CharacterActionPacket action, int index)
        {
            image.sprite = action.ActionImage;
            nameText.text = $"[{index}]\n{action.ActionName}";
        }
    }
}