using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] private List<Transform> speakers = new List<Transform>();
    public List<Transform> Speakers { get { return speakers; } }

    [SerializeField] private List<AudioClip> speakerBeeps = new List<AudioClip>();
    public List<AudioClip> SpeakerBeeps { get { return speakerBeeps; } }

    [SerializeField] private List<Dialogue> dialogues = new List<Dialogue>();
    public List<Dialogue> Dialogues { get { return dialogues; } }

    public void SetSpeakers(List<Transform> speakers)
    {
        this.speakers = speakers;
    }
}
