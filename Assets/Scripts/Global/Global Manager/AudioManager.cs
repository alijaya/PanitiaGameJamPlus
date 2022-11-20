using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using System;

public class AudioManager : SingletonMB<AudioManager>
{
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public Dictionary<AudioSource, float> lastTime = new Dictionary<AudioSource, float>();

    public FloatReference bgmVolume = new FloatReference(1);

    public FloatReference sfxVolume = new FloatReference(1);

    protected override void SingletonAwakened()
    {
        bgmVolume.GetEvent<FloatEvent>().Register(UpdateBGMVolume);
        UpdateBGMVolume();

        sfxVolume.GetEvent<FloatEvent>().Register(UpdateSFXVolume);
        UpdateSFXVolume();
    }

    private void UpdateBGMVolume()
    {
        bgmSource.volume = bgmVolume.Value;
    }

    private void UpdateSFXVolume()
    {
        sfxSource.volume = sfxVolume.Value;
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    public void PlayBGM(AudioClip clip, bool playFromBeginning = false, bool loop = true)
    {
        if (clip != bgmSource.clip)
        {
            lastTime[bgmSource] = bgmSource.time;
            bgmSource.clip = clip;
            if (!playFromBeginning && lastTime.TryGetValue(bgmSource, out var time))
            {
                bgmSource.time = time;
            }
            bgmSource.Play();
        }
    }
}
