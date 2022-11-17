using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;
    private float musicVolume = 1f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        UpdateVolume(Settings.Instance.musicVolume);
    }

    public void UpdateVolume(float value)
    {
        musicVolume = value;

        audioSource.volume = musicVolume;
        Settings.Instance.musicVolume = musicVolume;

        Settings.Save();
    }
}
