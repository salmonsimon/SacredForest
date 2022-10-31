using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSFX : SFX
{
    [SerializeField] private List<AudioClip> jumpingClips;

    [SerializeField] private List<AudioClip> meleeAttackClips;

    [SerializeField] private List<AudioClip> deathClips;

    public void PlayRandomJumpAudioClip()
    {
        if (jumpingClips.Count > 0)
            PlayRandomAudioClip(jumpingClips);
    }

    public void PlayRandomMeleeAudioClip()
    {
        if (meleeAttackClips.Count > 0)
            PlayRandomAudioClip(meleeAttackClips);
    }

    public void PlayRandomDeathAudioClip()
    {
        if (deathClips.Count > 0)
            PlayRandomAudioClip(deathClips);
    }
}
