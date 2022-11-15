using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Cutscene02 : Cutscene
{
    private Transform foxyDialogueTransform;
    private Transform akateDialogueTransform;

    protected override void Start()
    {
        base.Start();

        foxyDialogueTransform = GameObject.Find("Foxy - Intro/DialogueBubbleTransform").transform;
        akateDialogueTransform = GameManager.instance.GetPlayer().transform.Find("DialogueBubbleTransform").transform;
        speakers = new List<Transform>() { foxyDialogueTransform, akateDialogueTransform };

        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        GameManager.instance.GetLevelLoader().CinematicBracketsStart();

        GameObject player = GameManager.instance.GetPlayer();

        DeactivatePlayer(player);

        GameObject foxy = GameObject.FindWithTag("Foxy");
        foxy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        foxy.GetComponent<FoxyController>().StayInPosition();
        foxy.GetComponent<FoxyController>().enabled = false;

        #region First Portal Cutscene

        playableDirector.playableAsset = timelines[0];

        Bind(playableDirector, "Foxy Animations", foxy);
        Bind(playableDirector, "Akate Animations", player);

        playableDirector.Play();

        yield return new WaitForSeconds(1f);

        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[0], speakers);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        if (!GameManager.instance.GetDialogueManager().IsRunning && playableDirector.state == PlayState.Playing)
            playableDirector.Stop();

        #endregion

        ActivatePlayer(player);
        foxy.AddComponent<NPC>();

        StartCoroutine(GameManager.instance.GetLevelLoader().CinematicBracketsEnd());
        this.enabled = false;
    }
}
