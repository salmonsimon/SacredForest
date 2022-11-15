using UnityEngine;

[System.Serializable]
public struct Dialogue
{
    [SerializeField] private string title;
    public string Title { get { return title; } set { title = value; } }

    [SerializeField][TextArea(1,5)] private string text;
    public string Text { get { return text; } set { text = value; } }

    [SerializeField] private float duration;
    public float Duration { get { return duration; } set { duration = value; } }

    [SerializeField] private bool isSkippable;
    public bool IsSkippable { get { return isSkippable; } set { isSkippable = value; } }

    [SerializeField] private int speakerIndex;
    public int SpeakerIndex { get { return speakerIndex; } set { speakerIndex = value; } }

    [SerializeField] private bool isEnemyBubble;
    public bool IsEnemyBubble { get { return isEnemyBubble; } set { isEnemyBubble = value; } }

    // can add more to it later... like effects, changes in animations, etc
}