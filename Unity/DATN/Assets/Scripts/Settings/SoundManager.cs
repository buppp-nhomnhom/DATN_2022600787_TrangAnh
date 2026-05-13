using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip footstepClip; 
    public AudioClip pushClip; 
    public AudioClip lightSnowClip;
    public AudioClip heavySnowClip;
    public AudioClip openGateClip;

    public void PlayOpenGate()
    {
        if (openGateClip != null)
            audioSource.PlayOneShot(openGateClip);
    }

    public void PlayLightSnow()
    {
        if (lightSnowClip != null && !audioSource.isPlaying)
            audioSource.PlayOneShot(lightSnowClip);
    }

    public void PlayHeavySnow()
    {
        if (heavySnowClip != null && !audioSource.isPlaying)
            audioSource.PlayOneShot(heavySnowClip);
    }

    public void PlayPush()
    {
        if (pushClip != null)
            audioSource.PlayOneShot(pushClip);
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource.mute = !IsSoundEnabled();
    }

    public void PlayFootstep()
    {
        if (footstepClip != null)
            audioSource.PlayOneShot(footstepClip);
    }
    public void SetSound(bool isOn)
    {
        audioSource.mute = !isOn;
        PlayerPrefs.SetInt("SoundEnabled", isOn ? 1 : 0);
    }

    public bool IsSoundEnabled()
    {
        return PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
    }
} 