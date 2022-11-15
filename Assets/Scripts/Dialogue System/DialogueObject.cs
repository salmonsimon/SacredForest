using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] private List<Transform> speakers = new List<Transform>();
    public List<Transform> Speakers { get { return speakers; } }

    [SerializeField] private List<Dialogue> dialogues = new List<Dialogue>();
    public List<Dialogue> Dialogues { get { return dialogues; } }

    public void SetSpeakers(List<Transform> speakers)
    {
        this.speakers = speakers;
    }
}
