using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controller
{
    public class ActionCell : MonoBehaviour
    {
        private Image _image;
        private TextMeshProUGUI _nameText;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _nameText = GetComponent<TextMeshProUGUI>();
        }

        public void SetTo(CharacterAction action, int index)
        {
            _image.sprite = action.ActionImage;
            _nameText.text = $"[{index}]\n{action.ActionName}";
        }
    }
}