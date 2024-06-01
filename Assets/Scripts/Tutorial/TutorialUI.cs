using TMPro;
using UnityEngine;

namespace Tutorial
{
    public class TutorialUI : MonoBehaviour
    {
        [SerializeField] private TutorialTextAsset text;
        [SerializeField] private TextMeshProUGUI displayText;

        private int current;

        public void Next()
        {
            displayText.text = text.Get(current);
            current++;
        }
    }
}