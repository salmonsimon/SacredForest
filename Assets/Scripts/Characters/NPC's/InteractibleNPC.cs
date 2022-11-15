using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InteractibleNPC : NPC
{
    [SerializeField] private DialogueObject unclearedDialogues;
    [SerializeField] private DialogueObject clearedDialogues;
    private DialogueObject dialogues;

    [SerializeField] private List<Transform> speakers;

    [SerializeField] private BoxCollider playerCheck;

    [SerializeField] FightingRoute fightingRoute;

    private List<int> randomizedDialoguesIndexList;
    private int currentDialogueIndex = 0;

    private float lastTimeShownSurprise = 0;

    protected override void Start()
    {
        base.Start();

        bool clearedFightingRoute = false;

        switch (fightingRoute)
        {
            case FightingRoute.None:
                clearedFightingRoute = true;
                break;

            case FightingRoute.Route1:

                if (GameManager.instance.GetCurrentProgressManager().FinishedRoute1)
                    clearedFightingRoute = true;

                break;

            case FightingRoute.Route2:

                if (GameManager.instance.GetCurrentProgressManager().FinishedRoute2)
                    clearedFightingRoute = true;

                break;

            case FightingRoute.Route3:

                if (GameManager.instance.GetCurrentProgressManager().FinishedRoute3)
                    clearedFightingRoute = true;

                break;

            case FightingRoute.Route4:

                if (GameManager.instance.GetCurrentProgressManager().FinishedRoute4)
                    clearedFightingRoute = true;

                break;
        }

        if (clearedFightingRoute)
            dialogues = clearedDialogues;
        else
            dialogues = unclearedDialogues;

        dialogues.SetSpeakers(speakers);

        if (dialogues.Dialogues.Count > 0)
        {
            randomizedDialoguesIndexList = Enumerable.Range(1, dialogues.Dialogues.Count).ToList();
            randomizedDialoguesIndexList.Shuffle();
        }
        
    }

    protected override void Update()
    {
        base.Update();

        if (playerCheck != null)
        {
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

                    GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues, speakers, dialogueToPlay, dialogueToPlay);

                    if (currentDialogueIndex == dialogues.Dialogues.Count - 1)
                        currentDialogueIndex = 0;
                    else
                        currentDialogueIndex++;
                }
            }
        }
    }
}
