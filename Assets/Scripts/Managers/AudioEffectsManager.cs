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
        
        public static AudioEffectsManager Instance { get; private set; }
        
        public enum AudioEffect
        {
            Click, Attack, Move, Evil, Heal, Hit, Death, Ticking, Steps, Victory
        }
        
        private void Awake()
        {
            if (Instance is not null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            src = GetComponent<AudioSource>();
        }
        
        /// <summary>
        /// Plays a Clip of the given SoundEffect and returns its length in seconds
        /// </summary>
        /// <param name="effect">The ClipType to Play</param>
        /// <returns>The length of the played Clip</returns>
        public float PlayEffect(AudioEffect effect)
        {
            foreach (AudioClipsContainer cnt in clips)
            {
                if (cnt.Type != effect) continue;
                var clip = cnt.GetClip();
                src.PlayOneShot(clip);
                return clip.length;
            }
            
            Debug.LogError($"Sound with {effect} Identifier not found");
            return -1f;
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

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}