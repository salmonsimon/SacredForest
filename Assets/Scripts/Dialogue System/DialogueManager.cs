using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueManager : MonoBehaviour
{
    private GameObject dialogueContainer;

    [SerializeField] DialogueBubble whiteDialogueBubblePrefab;
    [SerializeField] DialogueBubble blackDialogueBubblePrefab;

    private List<DialogueBubble> dialogueBubbles = new List<DialogueBubble>();

    [SerializeField] private DialogueObject testDialogue;

    private bool isRunning = false;
    public bool IsRunning { get { return isRunning; } private set { isRunning = value; } }

    private void Start()
    {
        dialogueContainer = transform.gameObject;

        GameObject player = GameManager.instance.GetPlayer();
    }

    private void Update()
    {
        foreach (DialogueBubble dialogueBubble in dialogueBubbles)
            dialogueBubble.UpdateDialogueBubble();
    }

    public void RunBubbleDialogue(DialogueObject dialogueObject, List<Transform> speakers, int fromIndex = 0, int toIndex = -1)
    {
        dialogueObject.SetSpeakers(speakers);

        StartCoroutine(StepThroughDialogue(dialogueObject, fromIndex, toIndex));
    }

    private DialogueBubble GetDialogueBubble(bool isEnemyBubble)
    {
        DialogueBubble bubblePrefab = null;

        if (!isEnemyBubble)
            bubblePrefab = whiteDialogueBubblePrefab;
        else
            bubblePrefab = blackDialogueBubblePrefab;

        DialogueBubble dialogueBubble = Instantiate(bubblePrefab, dialogueContainer.transform);
        dialogueBubbles.Add(dialogueBubble);

        return dialogueBubble;
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject, int fromIndex = 0, int toIndex = -1)
    {
        DialogueBubble dialogueBubble = null;
        int previousSpeakerIndex = -1;
        bool openedBubble = false;

        if (fromIndex > dialogueObject.Dialogues.Count - 1)
            yield break;

        if (toIndex == -1 || toIndex > dialogueObject.Dialogues.Count - 1)
            toIndex = dialogueObject.Dialogues.Count - 1;

        isRunning = true;

        for (int i = fromIndex; i <= toIndex; i++)
        {
            Dialogue dialogue = dialogueObject.Dialogues[i];

            if (previousSpeakerIndex != -1)
            {
                if (previousSpeakerIndex != dialogue.SpeakerIndex)
                {
                    dialogueBubbles.Remove(dialogueBubble);
                    StartCoroutine(dialogueBubble.CloseDialogueBubble());
                    openedBubble = false;

                    yield return new WaitForSeconds(Config.BIG_DELAY);
                    yield return new WaitForSeconds(dialogueBubble.AnimationDuration);
                }
            }

            if (!openedBubble)
            {
                dialogueBubble = GetDialogueBubble(dialogue.IsEnemyBubble);

                dialogueBubble.initialPosition = dialogueObject.Speakers[dialogue.SpeakerIndex].parent.transform.position;
                dialogueBubble.finalPosition = dialogueObject.Speakers[dialogue.SpeakerIndex].transform.position;

                dialogueBubble.lookAt = dialogueObject.Speakers[dialogue.SpeakerIndex].transform;

                StartCoroutine(dialogueBubble.OpenDialogueBubble(dialogue.Text.Length));
                openedBubble = true;

                yield return new WaitForSeconds(Config.LARGE_DELAY);

                yield return new WaitForSeconds(Config.BIG_DELAY);
                yield return new WaitForSeconds(dialogueBubble.AnimationDuration);
            }

            previousSpeakerIndex = dialogue.SpeakerIndex;

            yield return RunTypingEffect(dialogue, dialogueBubble);

            dialogueBubble.TextLabel.text = dialogue.Text;

            yield return new WaitUntil(() => !dialogueBubble.TypewriterEffect.IsRunning);

            yield return new WaitForSeconds(Config.SMALL_DELAY);
            dialogueBubble.ShowSpaceBarIcon();

            yield return null;

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || dialogueBubble.TypewriterEffect.FinishedWaitingTime);

            dialogueBubble.TypewriterEffect.Reset();
            dialogueBubble.ShowSpaceBarIcon(false);
        }

        dialogueBubbles.Remove(dialogueBubble);
        StartCoroutine(dialogueBubble.CloseDialogueBubble());
        isRunning = false;
    }

    private IEnumerator RunTypingEffect(Dialogue dialogue, DialogueBubble dialogueBubble)
    {

        dialogueBubble.TypewriterEffect.Run(dialogue, dialogueBubble.TextLabel);

        while (dialogueBubble.TypewriterEffect.IsRunning)
        {
            yield return null;

            if (dialogue.IsSkippable && Input.GetKeyDown(KeyCode.Space))
            {
                dialogueBubble.TypewriterEffect.Stop();
            }
        }
    }
}