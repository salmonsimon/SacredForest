using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip grabSFX, throwSFX, storeSFX;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        grabSFX = Resources.Load<AudioClip>("Audio/Sound FX/Grab");
        throwSFX = Resources.Load<AudioClip>("Audio/Sound FX/Throw");
        storeSFX = Resources.Load<AudioClip>("Audio/Sound FX/Store");
    }

    // GameManager.instance.GetSoundManager().PlaySound(Config.HOVER_SFX);

    public void PlaySound(string str)
    {
        /*
        switch (str)
        {
            case Config.GRAB_SFX:
                audioSource.PlayOneShot(grabSFX);
                break;

            case Config.THROW_SFX:
                audioSource.PlayOneShot(throwSFX);
                break;

            case Config.STORE_SFX:
                audioSource.PlayOneShot(storeSFX);
                break;
        }
        */
    }
}
