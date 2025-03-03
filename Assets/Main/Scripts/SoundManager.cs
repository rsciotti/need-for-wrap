using UnityEngine;

namespace Main.Scripts
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        private AudioSource _audioSource;
        private AudioSource _musicSource;

        private void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this;
                _audioSource = GetComponents<AudioSource>()[0];
                _musicSource = GetComponents<AudioSource>()[1];
            }
        }

        public void PlaySoundEffect(AudioClip clip)
        {
            _audioSource.PlayOneShot(clip);
        }

        public void PlayMusic(AudioClip clip)
        {
            _musicSource.Stop();
            _musicSource.clip = clip;
            _musicSource.Play();
        }
    }
}