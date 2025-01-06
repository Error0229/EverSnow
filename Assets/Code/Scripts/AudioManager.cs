using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine.Audio;
using System;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource sfxObject;
    [SerializeField] List<string> sfxNames;
    [SerializeField] List<AudioClip> sfxClips;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private List<string> musicNames;
    [SerializeField] private List<AudioClip> musicClip;

    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 0.6f)
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
    public void PlaySFX(AudioClip clip, float volume = 0.6f)
    {
        if (clip != null)
        {
            AudioSource audioSource = Instantiate(sfxObject, Vector3.zero, Quaternion.identity);
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
            int index = sfxNames.IndexOf(soundEffectName);
            PlaySFX(sfxClips[index], volume);
        }
    }
    public void PlaySFX(string soundEffectName, Vector3 position, float volume = 0.6f)
    {
        if (string.IsNullOrEmpty(soundEffectName)) return;
        if (sfxNames.Contains(soundEffectName))
        {
            int index = sfxNames.IndexOf(soundEffectName);
            PlaySFX(sfxClips[index], position, volume);
        }
    }
    public void PlayMusic(AudioClip clip, float volume = 0.2f)
    {
        if (clip != null)
        {
            if (musicAudioSource.isPlaying)
            {
                StartCoroutine(FadeOutAndPlayNew(clip, volume, 0.1f));
            }
            else
            {
                musicAudioSource.volume = volume;
                musicAudioSource.clip = clip;
                StartCoroutine(FadeInMusic(0.1f));
            }
        }
    }
    private IEnumerator FadeOutAndPlayNew(AudioClip newClip, float targetVolume, float duration)
    {
        yield return StartCoroutine(FadeOutMusic(duration));
        musicAudioSource.clip = newClip;
        musicAudioSource.volume = targetVolume;
        yield return StartCoroutine(FadeInMusic(duration));
    }
    private IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = musicAudioSource.volume;
        while (musicAudioSource.volume > 0)
        {
            musicAudioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }
        musicAudioSource.Stop();
        musicAudioSource.volume = startVolume;
    }
    private IEnumerator FadeInMusic(float duration)
    {
        float startVolume = musicAudioSource.volume;
        musicAudioSource.volume = 0;
        musicAudioSource.Play();
        while (musicAudioSource.volume < startVolume)
        {
            musicAudioSource.volume += startVolume * Time.deltaTime / duration;
            yield return null;
        }
        musicAudioSource.volume = startVolume;
    }
    public void PlayMusic(string musicName, float volume = 0.2f)
    {
        if (string.IsNullOrEmpty(musicName)) return;
        if (musicNames.Contains(musicName))
        {
            int index = musicNames.IndexOf(musicName);
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
