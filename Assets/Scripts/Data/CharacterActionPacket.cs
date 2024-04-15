using System.Collections;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "CharacterAction")]
    public class CharacterActionPacket : ScriptableObject
    {
        public enum ActionType
        {
            Move, Attack, Help, BeEvil
        }
        
        public Sprite ActionImage => actionImage;
        [SerializeField] private Sprite actionImage;
        
        public string ActionName => actionName;
        [SerializeField] private string actionName;

        public string ActionDescription => actionDescription;
        [SerializeField] private string actionDescription;

        public ActionType Action => action;
        [SerializeField] private ActionType action;
    }
}