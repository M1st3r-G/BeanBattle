using UnityEngine;

namespace Data
{
    public class CharacterAction : ScriptableObject
    {
        public Sprite ActionImage => actionImage;
        [SerializeField] private Sprite actionImage;
        
        public string ActionName => actionName;
        [SerializeField] private string actionName;
    }
}