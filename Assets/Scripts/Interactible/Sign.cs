using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [SerializeField] private DialogueObject dialogue;
    [SerializeField] private BoxCollider playerCheck;

    [SerializeField] private bool isBreakableSign = true;
    private bool isBroken = false;

    private void Start()
    {
        if (isBreakableSign)
            GetComponent<DamageReceiver>().OnCharacterAliveStatusChange += SignBroken;
    }

    private void Update()
    {
        if (!isBroken && playerCheck != null)
        {
            if (!GameManager.instance.GetDialogueManager().IsRunning && playerCheck.IsColliding())
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // TODO: Change later for canvas dialogue
                    GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogue, new List<Transform>() { transform });
                }
            }
        }
    }

    private void SignBroken()
    {
        if (!GetComponent<DamageReceiver>().IsAlive)
            isBroken = true;
    }
}
