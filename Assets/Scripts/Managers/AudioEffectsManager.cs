using UnityEngine;

namespace Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioEffectsManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] clips;
        private AudioSource src;

        public enum AudioEffect
        {
            Debug
        }
        
        private void Awake()
        {
            src = GetComponent<AudioSource>();
        }

        public void PlayEffect(AudioEffect effect)
        {
            src.PlayOneShot(clips[(int)effect]);
        }
    }
}