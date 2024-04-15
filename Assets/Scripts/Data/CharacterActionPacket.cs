using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "CharacterAction")]
    public class CharacterActionPacket : ScriptableObject
    {
        public Sprite ActionImage => actionImage;
        [SerializeField] private Sprite actionImage;
        
        public string ActionName => actionName;
        [SerializeField] private string actionName;

        public string ActionDescription => actionDescription;
        [SerializeField] private string actionDescription;
    }
}