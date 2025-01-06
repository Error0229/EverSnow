using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource sfxObject;
    [SerializeField]
    private List<string> sfxNames;
    [SerializeField]
    private List<AudioClip> sfxClips;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private List<string> musicNames;
    [SerializeField] private List<AudioClip> musicClip;

    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 0.6f)
    {
        if (clip != null)
        {
            var audioSource = Instantiate(sfxObject, position, Quaternion.identity);
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
            Destroy(audioSource.gameObject, clip.length);
        }
    }

    // Add new method for UI sounds
    public void PlaySFX(AudioClip clip, float volume = 0.6f)
    {
        if (clip != null)
        {
            var audioSource = Instantiate(sfxObject, Vector3.zero, Quaternion.identity);
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.spatialBlend = 0.0f;
            audioSource.Play();
            Destroy(audioSource.gameObject, clip.length);
        }
        PlaySFX(clip, Vector3.zero, volume);
    }

    public void PlaySFX(string soundEffectName, float volume = 0.6f)
    {
        if (string.IsNullOrEmpty(soundEffectName)) return;
        if (sfxNames.Contains(soundEffectName))
        {
            var index = sfxNames.IndexOf(soundEffectName);
            PlaySFX(sfxClips[index], volume);
        }
    }
    public void PlaySFX(string soundEffectName, Vector3 position, float volume = 0.6f)
    {
        if (string.IsNullOrEmpty(soundEffectName)) return;
        if (sfxNames.Contains(soundEffectName))
        {
            var index = sfxNames.IndexOf(soundEffectName);
            PlaySFX(sfxClips[index], position, volume);
        }
    }
    public void PlayMusic(AudioClip clip, float volume = 0.2f)
    {
        if (clip)
        {
            if (musicAudioSource.isPlaying)
            {
                musicAudioSource.Stop();
            }
            musicAudioSource.volume = volume;
            musicAudioSource.clip = clip;
            musicAudioSource.Play();
        }
    }
    public void PlayMusic(string musicName, float volume = 0.2f)
    {
        if (string.IsNullOrEmpty(musicName)) return;
        if (musicNames.Contains(musicName))
        {
            var index = musicNames.IndexOf(musicName);
            PlayMusic(musicClip[index], volume);
        }
    }
    public void StopMusic()
    {
        musicAudioSource.Stop();
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20);
    }

    public void SetSFXVolume(float level)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(level) * 20);
    }
    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20);
    }

    protected override void Init()
    {
        base.Init();
        SetMasterVolume(0.5f);
        SetMusicVolume(0.5f);
        SetSFXVolume(0.5f);
    }
}
