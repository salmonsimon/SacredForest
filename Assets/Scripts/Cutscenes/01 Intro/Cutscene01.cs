using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Cutscene01 : Cutscene
{
    private Transform foxyDialogueTransform;
    private Transform akateDialogueTransform;

    protected override void Start()
    {
        base.Start();

        if (GameManager.instance.GetCurrentProgressManager().FirstTimePlaying)
        {
            foxyDialogueTransform = GameObject.Find("Foxy - Intro/DialogueBubbleTransform").transform;
            akateDialogueTransform = GameManager.instance.GetPlayer().transform.Find("DialogueBubbleTransform").transform;
            speakers = new List<Transform>() { foxyDialogueTransform, akateDialogueTransform };

            StartCoroutine(Play());
        }
        else
            Destroy(this.gameObject);
    }

    private IEnumerator Play()
    {
        GameManager.instance.GetLevelLoader().CinematicBracketsStart();

        GameObject player = GameManager.instance.GetPlayer();

        DeactivatePlayer(player);

        RuntimeAnimatorController playerAnimator = player.GetComponent<Animator>().runtimeAnimatorController;
        player.GetComponent<Animator>().runtimeAnimatorController = null;

        GameObject foxy = GameObject.FindWithTag("Foxy");

        #region Game Intro Cutscene - Intro

        playableDirector.playableAsset = timelines[0];

        Bind(playableDirector, "Foxy Animations", foxy);
        Bind(playableDirector, "Akate Animations", player);
        Bind(playableDirector, "Crossfade Animation", GameManager.instance.GetLevelLoader().GetCrossfadeAnimator().gameObject);

        playableDirector.extrapolationMode = DirectorWrapMode.Loop;
        playableDirector.Play();

        yield return new WaitForSeconds(3f);
        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[0], speakers, 0, 3);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        if (!GameManager.instance.GetDialogueManager().IsRunning && playableDirector.state == PlayState.Playing)
            playableDirector.Stop();

        #endregion

        #region Game Intro Cutscene - Akate Wakes Up

        yield return new WaitForSeconds(Config.MEDIUM_DELAY);

        playableDirector.playableAsset = timelines[1];

        Bind(playableDirector, "Foxy Animations", foxy);
        Bind(playableDirector, "Foxy Movements", foxy);
        Bind(playableDirector, "Akate Animations", player);

        playableDirector.extrapolationMode = DirectorWrapMode.None;
        playableDirector.Play();

        yield return new WaitForSeconds(1f);

        player.GetComponent<Animator>().runtimeAnimatorController = playerAnimator;

        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[0], speakers, 4);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        #endregion

        #region Game Intro Cutscene - Akate Starts Following

        yield return new WaitForSeconds(Config.SMALL_DELAY);

        playableDirector.playableAsset = timelines[2];

        Bind(playableDirector, "Foxy Animations", foxy);
        Bind(playableDirector, "Foxy Movements", foxy);

        foxy.GetComponent<SpriteRenderer>().flipX = false;

        playableDirector.Play();

        yield return new WaitForSeconds(1f);

        #endregion

        foxy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        foxy.GetComponent<FoxyController>().enabled = true;

        ActivatePlayer(player);

        GameManager.instance.GetAnimationManager().ShowImageUIWorldSpace(Config.MOVEMENT_KEYS_GUI,
            new Vector3(player.transform.position.x, player.transform.position.y + .32f, player.transform.position.z), player.transform, 6);

        StartCoroutine(GameManager.instance.GetLevelLoader().CinematicBracketsEnd());

        yield return new WaitForSeconds(1.2f);

        GameManager.instance.GetAnimationManager().ShowImageUI(Config.RIGHT_ARROW_GUI, true);

        this.enabled = false;
    }
}
