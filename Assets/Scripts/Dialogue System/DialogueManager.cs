using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DialogueManager : MonoBehaviour
{
    #region World Position Canvas

    [Header("World Position Canvas")]

    [SerializeField] private GameObject worldPositionCanvas;

    [SerializeField] DialogueBubble whiteDialogueBubblePrefab;
    [SerializeField] DialogueBubble blackDialogueBubblePrefab;

    private List<DialogueBubble> dialogueBubbles = new List<DialogueBubble>();

    #endregion

    #region Screen Overlay Canvas

    [Space(10)]
    [Header("Screen Overlay Canvas")]
    [Space(2)]

    [SerializeField] private DialogueOverlay screenOverlayDialoguePanel;

    [SerializeField] private DialogueObject deathDialogues;
    private List<int> randomizedDeathDialoguesIndexList;
    private int currentDeathDialogueIndex = 0;

    #endregion

    private bool isRunning = false;
    public bool IsRunning { get { return isRunning; } private set { isRunning = value; } }

    private void Start()
    {
        GameObject player = GameManager.instance.GetPlayer();

        screenOverlayDialoguePanel.gameObject.SetActive(false);

        if (deathDialogues.Dialogues.Count > 0)
        {
            randomizedDeathDialoguesIndexList = Enumerable.Range(1, deathDialogues.Dialogues.Count).ToList();
            randomizedDeathDialoguesIndexList.Shuffle();
        }
    }

    private void Update()
    {
        foreach (DialogueBubble dialogueBubble in dialogueBubbles)
            dialogueBubble.UpdateDialogueBubble();
    }

    public void RunScreenOverlayDialogue(DialogueObject dialogueObject, int fromIndex = 0, int toIndex = -1, bool showSpaceBar = true)
    {
        StartCoroutine(StepThroughOverlayDialogue(dialogueObject, fromIndex, toIndex, showSpaceBar));
    }

    public void RunBubbleDialogue(DialogueObject dialogueObject, List<Transform> speakers, int fromIndex = 0, int toIndex = -1)
    {
        dialogueObject.SetSpeakers(speakers);

        StartCoroutine(StepThroughBubbleDialogue(dialogueObject, fromIndex, toIndex));
    }

    private DialogueBubble GetDialogueBubble(bool isEnemyBubble)
    {
        DialogueBubble bubblePrefab = null;

        if (!isEnemyBubble)
            bubblePrefab = whiteDialogueBubblePrefab;
        else
            bubblePrefab = blackDialogueBubblePrefab;

        DialogueBubble dialogueBubble = Instantiate(bubblePrefab, worldPositionCanvas.transform);
        dialogueBubbles.Add(dialogueBubble);

        return dialogueBubble;
    }

    private IEnumerator StepThroughBubbleDialogue(DialogueObject dialogueObject, int fromIndex = 0, int toIndex = -1)
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

            yield return RunTypingEffect(dialogue, dialogueBubble, dialogueObject.SpeakerBeeps[dialogue.SpeakerIndex]);

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

    private IEnumerator StepThroughOverlayDialogue(DialogueObject dialogueObject, int fromIndex = 0, int toIndex = -1, bool showSpaceBar = true)
    {
        if (fromIndex > dialogueObject.Dialogues.Count - 1)
            yield break;

        if (toIndex == -1 || toIndex > dialogueObject.Dialogues.Count - 1)
            toIndex = dialogueObject.Dialogues.Count - 1;

        isRunning = true;

        for (int i = fromIndex; i <= toIndex; i++)
        {
            Dialogue dialogue = dialogueObject.Dialogues[i];

            screenOverlayDialoguePanel.gameObject.SetActive(true);

            if (!screenOverlayDialoguePanel.IsOpen)
            {
                StartCoroutine(screenOverlayDialoguePanel.OpenDialogueOverlay());

                yield return new WaitForSeconds(Config.LARGE_DELAY);

                yield return new WaitForSeconds(Config.BIG_DELAY);
                yield return new WaitForSeconds(screenOverlayDialoguePanel.AnimationDuration);
            }

            yield return RunTypingEffect(dialogue, screenOverlayDialoguePanel, dialogueObject.SpeakerBeeps[dialogue.SpeakerIndex]);

            screenOverlayDialoguePanel.TextLabel.text = dialogue.Text;

            yield return new WaitUntil(() => !screenOverlayDialoguePanel.TypewriterEffect.IsRunning);

            yield return new WaitForSeconds(Config.SMALL_DELAY);

            if (showSpaceBar)
                screenOverlayDialoguePanel.ShowSpaceBarIcon();
            else
                isRunning = false;

            yield return null;

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || screenOverlayDialoguePanel.TypewriterEffect.FinishedWaitingTime);

            screenOverlayDialoguePanel.TypewriterEffect.Reset();
            screenOverlayDialoguePanel.ShowSpaceBarIcon(false);
        }

        StartCoroutine(screenOverlayDialoguePanel.CloseDialogueOverlay());
        isRunning = false;
    }

    private IEnumerator RunTypingEffect(Dialogue dialogue, DialogueBubble dialogueBubble, AudioClip speakerBeep)
    {

        dialogueBubble.TypewriterEffect.Run(dialogue, dialogueBubble.TextLabel, speakerBeep, true);

        while (dialogueBubble.TypewriterEffect.IsRunning)
        {
            yield return null;

            if (dialogue.IsSkippable && Input.GetKeyDown(KeyCode.Space))
            {
                dialogueBubble.TypewriterEffect.Stop();
            }
        }
    }

    private IEnumerator RunTypingEffect(Dialogue dialogue, DialogueOverlay screenOverlayDialoguePanel, AudioClip speakerBeep)
    {

        screenOverlayDialoguePanel.TypewriterEffect.Run(dialogue, screenOverlayDialoguePanel.TextLabel, speakerBeep, false);

        while (screenOverlayDialoguePanel.TypewriterEffect.IsRunning)
        {
            yield return null;

            if (dialogue.IsSkippable && Input.GetKeyDown(KeyCode.Space))
            {
                screenOverlayDialoguePanel.TypewriterEffect.Stop();
            }
        }
    }

    public void ShowRandomDeathDialogue()
    {
        int dialogueToPlay = randomizedDeathDialoguesIndexList[currentDeathDialogueIndex];

        GameManager.instance.GetDialogueManager().RunScreenOverlayDialogue(deathDialogues, dialogueToPlay, dialogueToPlay, false);

        if (currentDeathDialogueIndex == deathDialogues.Dialogues.Count - 1)
            currentDeathDialogueIndex = 0;
        else
            currentDeathDialogueIndex++;
    }

    public void ClearDialogues()
    {
        StopAllCoroutines();

        dialogueBubbles.Clear();

        foreach (Transform child in worldPositionCanvas.transform)
        {
            StartCoroutine(child.gameObject.GetComponent<DialogueBubble>().CloseDialogueBubble());
        }

        isRunning = false;
    }
}