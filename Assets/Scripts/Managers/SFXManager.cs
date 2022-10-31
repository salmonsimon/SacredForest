using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Projectile Sounds")]
    [SerializeField] private AudioClip arrowHitSFX;

    [Header("Damage Sounds")]
    [SerializeField] private List<AudioClip> swordDamageClips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> defaultDamageClips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> bludgeoningDamageClips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> fireDamageClips = new List<AudioClip>();

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        //arrowHitSFX = Resources.Load<AudioClip>("Audio/SFX/Projectile/Arrow/hit");
    }

    // GameManager.instance.GetSoundManager().PlaySound(Config.HOVER_SFX);

    public void PlaySound(string str)
    {
        switch (str)
        {
            case Config.ARROW_HIT_SFX:
                audioSource.PlayOneShot(arrowHitSFX);
                break;

            /*
            case Config.THROW_SFX:
                audioSource.PlayOneShot(throwSFX);
                break;

            case Config.STORE_SFX:
                audioSource.PlayOneShot(storeSFX);
                break;
            */
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
}
