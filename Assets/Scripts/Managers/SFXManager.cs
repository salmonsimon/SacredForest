using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip stepClip;

    [Header("Projectile Sounds")]
    [SerializeField] private AudioClip arrowHitSFX;

    [Header("Damage Sounds")]
    [SerializeField] private List<AudioClip> swordDamageClips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> defaultDamageClips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> bludgeoningDamageClips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> fireDamageClips = new List<AudioClip>();

    [Header("Step Sound")]
    [SerializeField] private List<AudioClip> grassStepClips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> rockStepClips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> woodStepClips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> dirtStepClips = new List<AudioClip>();

    private int currentGrassStepClipIndex = 0;
    private int currentRockStepClipsIndex = 0;
    private int currentWoodStepClipsIndex = 0;
    private int currentDirtStepClipsIndex = 0;

    [Header("Landing Sound")]
    [SerializeField] private AudioClip grassLandingClip;
    [SerializeField] private AudioClip rockLandingClip;
    [SerializeField] private AudioClip woodLandingClip;
    [SerializeField] private AudioClip dirtLandingClip;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();


    }

    // GameManager.instance.GetSoundManager().PlaySound(Config.HOVER_SFX);

    public void PlaySound(string str)
    {
        switch (str)
        {
            case Config.ARROW_HIT_SFX:
                audioSource.PlayOneShot(arrowHitSFX);
                break;

            case Config.STEP_SFX:
                audioSource.PlayOneShot(stepClip);
                break;
        }
    }

    public void PlaySound(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayRandomAudioClip(List<AudioClip> audioClips)
    {
        int randomClip = Random.Range(0, audioClips.Count);

        PlaySound(audioClips[randomClip]);
    }

    #region Damage Sounds

    public void PlayRandomSwordDamageClip()
    {
        PlayRandomAudioClip(swordDamageClips);
    }

    public void PlayRandomDefaultDamageClip()
    {
        PlayRandomAudioClip(defaultDamageClips);
    }

    public void PlayRandomBludgeoningDamageClip()
    {
        PlayRandomAudioClip(bludgeoningDamageClips);
    }

    public void PlayRandomFireDamageClip()
    {
        PlayRandomAudioClip(fireDamageClips);
    }

    #endregion

    #region Step and Landing Sounds

    public void PlayGrassStepSound()
    {
        PlaySound(grassStepClips[currentGrassStepClipIndex]);

        if (currentGrassStepClipIndex < grassStepClips.Count - 1)
            currentGrassStepClipIndex++;
        else
            currentGrassStepClipIndex = 0;
    }

    public void PlayGrassLandingSound()
    {
        PlaySound(grassLandingClip);
    }

    public void PlayRockStepSound()
    {
        PlaySound(rockStepClips[currentRockStepClipsIndex]);

        if (currentRockStepClipsIndex < rockStepClips.Count - 1)
            currentRockStepClipsIndex++;
        else
            currentRockStepClipsIndex = 0;
    }

    public void PlayRockLandingSound()
    {
        PlaySound(rockLandingClip);
    }

    public void PlayWoodStepSound()
    {
        PlaySound(woodStepClips[currentWoodStepClipsIndex]);

        if (currentWoodStepClipsIndex < woodStepClips.Count - 1)
            currentWoodStepClipsIndex++;
        else
            currentWoodStepClipsIndex = 0;
    }

    public void PlayWoodLandingSound()
    {
        PlaySound(woodLandingClip);
    }

    public void PlayDirtStepSound()
    {
        PlaySound(dirtStepClips[currentDirtStepClipsIndex]);

        if (currentDirtStepClipsIndex < dirtStepClips.Count - 1)
            currentDirtStepClipsIndex++;
        else
            currentDirtStepClipsIndex = 0;
    }

    public void PlayDirtLandingSound()
    {
        PlaySound(dirtLandingClip);
    }

    #endregion
}