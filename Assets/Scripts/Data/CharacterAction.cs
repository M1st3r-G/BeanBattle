using System.Text;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "CharacterAction")]
    public class CharacterAction : ScriptableObject
    {
        public enum ActionTypes
        {
            Move, Attack, BeEvil, Help    
        }
        
        public Sprite ActionImage => actionImage;
        [SerializeField] private Sprite actionImage;

        public string ActionName => AddSpacesToSentence(actionType.ToString());
        public ActionTypes Type => actionType;
        [SerializeField] private ActionTypes actionType;

        public string ActionDescription => actionDescription;
        [SerializeField] private string actionDescription;

        private static string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }
}