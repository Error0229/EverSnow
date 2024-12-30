using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;

public class SFXManager : Singleton<SFXManager>
{
    [SerializeField] private AudioSource sfxObject;
    [SerializeField] List<string> sfxNames;
    [SerializeField] List<AudioClip> sfxClips;

    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip != null)
        {
            AudioSource audioSource = Instantiate(sfxObject, position, Quaternion.identity);
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
            Destroy(audioSource.gameObject, clip.length);
        }
    }

    // Add new method for UI sounds
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        PlaySFX(clip, Vector3.zero, volume);
    }

    public void PlaySoundEffect(string soundEffectName, float volume = 1f)
    {
        PlaySoundEffect(soundEffectName, Vector3.zero, volume);
    }
    public void PlaySoundEffect(string soundEffectName, Vector3 position, float volume = 1f)
    {
        if (string.IsNullOrEmpty(soundEffectName)) return;
        if (sfxNames.Contains(soundEffectName))
        {
            int index = sfxNames.IndexOf(soundEffectName);
            PlaySFX(sfxClips[index], position, volume);
        }

    }
}
