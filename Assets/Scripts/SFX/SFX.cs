using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    public void PlayAudioClip(AudioClip audioClip)
    {
        GameManager.instance.GetSFXManager().PlaySound(audioClip);
    }

    public void PlayRandomAudioClip(List<AudioClip> audioClips)
    {
        int randomClip = Random.Range(0, audioClips.Count);

        GameManager.instance.GetSFXManager().PlaySound(audioClips[randomClip]);
    }
}
