using Controller;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIContent
{
    /// <summary>
    /// Internally used by the <see cref="UIManager"/> to Display the Name and Picture of the CurrentCharacter
    /// </summary>
    public class CurrentCharacterUI : MonoBehaviour
    {
        //ComponentReferences
        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI nameText; 
        
        public void SetCharacter(CharController newChar)
        {
            portrait.color = newChar.GetData.Material(newChar.TeamID).color;
            nameText.text = newChar.GetData.Name;
        }
    }
}