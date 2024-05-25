using System.Linq;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Manages all Information regarding a Character Action
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterAction")]
    public class CharacterAction : ScriptableObject
    {
        // Public
        public enum ActionTypes
        {
            None, Move, Attack, BeEvil, Help, Cover
        }

        #region Fields
        
        public Sprite ActionImage => actionImage;
        [SerializeField] private Sprite actionImage;

        public string ActionName => AddSpacesToSentence(actionType.ToString());
        public ActionTypes Type => actionType;
        [SerializeField] private ActionTypes actionType;

        public string ActionDescription => actionDescription;
        [SerializeField] private string actionDescription;
        
        #endregion

        private static string AddSpacesToSentence(string text) =>
            text.Aggregate("", (current, t) => current + (char.IsUpper(t) ? $" {t}" : t))[1..];
    }
}