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

    private void Start()
    {
        dialogueContainer = transform.gameObject;

        GameObject player = GameManager.instance.GetPlayer();

        // TODO: delete this ones
        //ShowDialogue(testDialogue, new Vector3(player.transform.position.x + .4f, player.transform.position.y + .4f, player.transform.position.z), player.transform, "BlackBubble");
        //ShowDialogue(testDialogue, new Vector3(player.transform.position.x + .4f, player.transform.position.y + .4f, player.transform.position.z), player.transform);
    }

    private void Update()
    {
        foreach (DialogueBubble dialogueBubble in dialogueBubbles)
            dialogueBubble.UpdateDialogueBubble();
    }

    public void ShowDialogue(DialogueObject dialogueObject, Vector3 position, Transform lookAt, string bubbleType = "WhiteBubble")
    {
        DialogueBubble dialogueBubble = GetDialogueBubble(bubbleType);

        dialogueBubble.finalPosition = position;

        if (lookAt)
        {
            dialogueBubble.lookAt = lookAt;
            dialogueBubble.originalDisplacement = position - lookAt.position;
        }

        StartCoroutine(StepThroughDialogue(dialogueObject, dialogueBubble));
    }

    private DialogueBubble GetDialogueBubble(string bubbleType)
    {
        DialogueBubble bubblePrefab = null;

        switch (bubbleType)
        {
            case Config.WHITE_DIALOGUE_BUBBLE:
                bubblePrefab = whiteDialogueBubblePrefab;
                break;

            case Config.BLACK_DIALOGUE_BUBBLE:
                bubblePrefab = blackDialogueBubblePrefab;
                break;
        }

        DialogueBubble dialogueBubble = dialogueBubbles.Find(x => !x.IsOpen);

        if (dialogueBubble == null)
        {
            dialogueBubble = Instantiate(bubblePrefab, dialogueContainer.transform);

            dialogueBubbles.Add(dialogueBubble);
        }

        return dialogueBubble;
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject, DialogueBubble dialogueBubble)
    {
        bool openedBubble = false;

        foreach (Dialogue dialogue in dialogueObject.Dialogues)
        {
            if (!openedBubble)
            {
                StartCoroutine(dialogueBubble.OpenDialogueBubble(dialogue.Text.Length));
                openedBubble = true;

                //TODO: dsps borrar este timer
                yield return new WaitForSeconds(2f);

                yield return new WaitForSeconds(Config.BIG_DELAY);
                yield return new WaitForSeconds(dialogueBubble.AnimationDuration);
            }

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

        StartCoroutine(dialogueBubble.CloseDialogueBubble());
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
