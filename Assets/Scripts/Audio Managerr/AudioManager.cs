using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Smarteye.AR
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private Sound[] musicSounds, sfxSounds;
        [SerializeField] private AudioSource musicSource, sfxSource;

        private void Start()
        {
            PlayBackgroundMusic("Theme");
        }

        public void PlayBackgroundMusic(string name)
        {
            Sound sound = FindSound(name, musicSounds);
            if (sound != null)
            {
                musicSource.clip = sound.clip;
                musicSource.loop = true;
                musicSource.Play();
            }
        }

        public void PlaySFX(string name)
        {
            Sound sound = FindSound(name, sfxSounds);
            if (sound != null)
            {
                sfxSource.PlayOneShot(sound.clip);
            }
        }

        public Sound FindSound(string name, Sound[] sounds)
        {
            foreach (var sound in sounds)
            {
                if (sound.name == name)
                {
                    return sound;
                }
            }

            Debug.LogWarning($"Audio Manager: {name} isn't available");
            return null;
        }
    }
}