using UnityEngine;

namespace Tutorial
{
    [CreateAssetMenu(menuName = "Tutorial/Text")]
    public class TutorialTextAsset : ScriptableObject
    {
        [SerializeField] private string[] content;

        public string Get(int i) => content[i];
    }
}