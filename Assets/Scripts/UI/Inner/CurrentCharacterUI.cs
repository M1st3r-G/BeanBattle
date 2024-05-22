using Controller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inner
{
    public class CurrentCharacterUI : MonoBehaviour
    {
        //ComponentReferences
        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI nameText; 
        //Temps
        //Public
        
        public void SetCharacter(CharController newChar)
        {
            portrait.color = newChar.GetData.Material(newChar.TeamID).color;
            nameText.text = newChar.GetData.Name;
        }
    }
}