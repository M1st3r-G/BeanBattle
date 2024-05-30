using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioEffectsManager : MonoBehaviour
    {
        [SerializeField] private AudioClipsContainer[] clips;
        private AudioSource src;

        public enum AudioEffect
        {
            Click, Attack, Move, Hit, Death, Evil, Heal
        }
        
        private void Awake()
        {
            src = GetComponent<AudioSource>();
        }

        public void PlayEffect(AudioEffect effect)
        {
            foreach (AudioClipsContainer cnt in clips)
            {
                if (cnt.Type == effect)
                {
                    src.PlayOneShot(cnt.GetClip());
                    return;
                }
            }
            
            Debug.LogError($"Sound with {effect} Identifier not found");
        }

        [Serializable]
        private struct AudioClipsContainer
        {
            public string name;
            [SerializeField] private AudioEffect type;
            [SerializeField] private AudioClip[] clips;

            public AudioEffect Type => type;
            
            public AudioClip GetClip()
            {
                return clips[Random.Range(0, clips.Length)];
            }
        }
    }
}