using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "CharacterAction")]
    public class CharacterAction : ScriptableObject
    {
        public Sprite ActionImage => actionImage;
        [SerializeField] private Sprite actionImage;
        
        public string ActionName => actionName;
        [SerializeField] private string actionName;
    }
}