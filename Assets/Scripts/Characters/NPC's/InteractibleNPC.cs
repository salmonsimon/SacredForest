using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InteractibleNPC : MonoBehaviour
{
    [SerializeField] private DialogueObject unclearedDialogues;
    [SerializeField] private DialogueObject clearedDialogues;
    private DialogueObject dialogues;

    [SerializeField] private List<Transform> speakers;

    [SerializeField] private BoxCollider playerCheck;

    private Vector3 originalScale;

    private List<int> randomizedDialoguesIndexList;
    private int currentDialogueIndex = 0;

    private float lastTimeShownSurprise = 0;

    private void Start()
    {
        originalScale = transform.localScale;

        if (ProgressManager.Instance.finishedRoute1)
            dialogues = clearedDialogues;
        else
            dialogues = unclearedDialogues;

        dialogues.SetSpeakers(speakers);

        randomizedDialoguesIndexList = Enumerable.Range(1, dialogues.Dialogues.Count).ToList();
        randomizedDialoguesIndexList.Shuffle();
    }

    private void Update()
    {
        float relativePlayerPositionX = GameManager.instance.GetPlayer().transform.position.x - transform.position.x;
        Flip(new Vector2(relativePlayerPositionX, 0));

        if (!GameManager.instance.GetDialogueManager().IsRunning && playerCheck.IsColliding())
        {
            if (lastTimeShownSurprise + 5f < Time.time)
            {
                GameManager.instance.ShowText("!", 1, Color.white, new Vector3(transform.position.x, transform.position.y + 0.32f, 0), Vector3.up * .05f, 2, transform);
                lastTimeShownSurprise = Time.time;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                int dialogueToPlay = randomizedDialoguesIndexList[currentDialogueIndex];

                GameManager.instance.GetDialogueManager().RunBubbleDialogue(unclearedDialogues, speakers, dialogueToPlay, dialogueToPlay);

                if (currentDialogueIndex == dialogues.Dialogues.Count - 1)
                    currentDialogueIndex = 0;
                else
                    currentDialogueIndex++;
            }
        }
    }

    public void Flip(Vector2 movement)
    {
        if (movement.x > 0)
        {
            if (originalScale.x > 0)
            {
                transform.localScale = originalScale;
            }
            else
            {
                transform.localScale = new Vector3(originalScale.x * -1f, originalScale.y, originalScale.z);
            }
        }
        else if (movement.x < 0)
        {
            if (originalScale.x > 0)
            {
                transform.localScale = new Vector3(originalScale.x * -1f, originalScale.y, originalScale.z);
            }
            else
            {
                transform.localScale = originalScale;
            }
        }
    }
}
