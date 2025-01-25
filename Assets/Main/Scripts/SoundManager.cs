using UnityEngine;

namespace Main.Scripts
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        private AudioSource _audioSource;

        private void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this;
                _audioSource = GetComponent<AudioSource>();
            }
        }

        public void PlaySoundEffect(AudioClip clip)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}