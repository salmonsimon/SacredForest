using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Cutscene04 : Cutscene
{
    private Transform enemyDialogueTransform;
    private GameObject enemy;

    protected override void Start()
    {
        base.Start();

        if (!GameManager.instance.GetCurrentProgressManager().FinishedRoute1)
            StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        GameManager.instance.GetLevelLoader().CinematicBracketsStart();

        yield return new WaitForSeconds(Config.MEDIUM_DELAY);

        GameObject player = GameManager.instance.GetPlayer();
        DeactivatePlayer(player);

        enemy = GameObject.FindGameObjectWithTag("Enemy");
        enemyDialogueTransform = enemy.transform.Find("DialogueBubbleHolder").transform;
        speakers = new List<Transform>() { enemyDialogueTransform };

        #region Ground Monk Introduction

        playableDirector.playableAsset = timelines[0];

        Bind(playableDirector, "Monk Animations", enemy);
        Bind(playableDirector, "Akate Animations", player);

        playableDirector.Play();

        yield return new WaitForSeconds(2f);

        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[0], speakers);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        if (!GameManager.instance.GetDialogueManager().IsRunning && playableDirector.state == PlayState.Playing)
            playableDirector.Stop();

        #endregion

        ActivatePlayer(player);

        yield return new WaitForSeconds(Config.SMALL_DELAY);

        enemy.GetComponent<Animator>().SetTrigger("Fight");
        enemy.GetComponent<GroundMonk>().enabled = true;

        StartCoroutine(GameManager.instance.GetLevelLoader().CinematicBracketsEnd());
    }

    public IEnumerator PlayTransformScene()
    {
        GameManager.instance.GetLevelLoader().CinematicBracketsStart();

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        GameObject player = GameManager.instance.GetPlayer();
        DeactivatePlayer(player);

        player.GetComponent<DamageReceiver>().enabled = false;

        enemy = GameObject.FindGameObjectWithTag("Enemy");
        GroundMonk groundMonk = enemy.GetComponent<GroundMonk>();
        groundMonk.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        enemyDialogueTransform = enemy.transform.Find("DialogueBubbleHolder").transform;
        speakers = new List<Transform>() { enemyDialogueTransform };

        #region Transformation Cutscene

        playableDirector.playableAsset = timelines[1];

        Bind(playableDirector, "Monk Animations", enemy);

        playableDirector.extrapolationMode = DirectorWrapMode.Hold;
        playableDirector.Play();

        yield return new WaitForSeconds(1f);

        GameManager.instance.GetDialogueManager().RunScreenOverlayDialogue(dialogues[1]);


        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        if (!GameManager.instance.GetDialogueManager().IsRunning && playableDirector.state == PlayState.Playing)
            playableDirector.Stop();

        playableDirector.extrapolationMode = DirectorWrapMode.None;

        #endregion

        GameManager.instance.GetCurrentProgressManager().Route1BossHasTransformed = true;

        StartCoroutine(GameManager.instance.GetLevelLoader().CinematicBracketsEnd());

        player.GetComponent<DamageReceiver>().enabled = true;

        yield return null;

        groundMonk.Transform();

        ActivatePlayer(player);

        yield return new WaitForSeconds(3.7f);

        groundMonk.enabled = true;
        groundMonk.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }

    public IEnumerator PlayDeathScene()
    {
        GameManager.instance.GetLevelLoader().CinematicBracketsStart();
        GameManager.instance.GetMusicManager().StopMusic();

        GameObject player = GameManager.instance.GetPlayer();
        DeactivatePlayer(player);

        player.GetComponent<DamageReceiver>().enabled = false;

        Transform playerDialogueTransform = player.transform.Find("DialogueBubbleTransform").transform;

        enemy = GameObject.FindGameObjectWithTag("Enemy");
        GroundMonk groundMonk = enemy.GetComponent<GroundMonk>();
        groundMonk.enabled = false;
        groundMonk.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        groundMonk.GetComponent<Animator>().runtimeAnimatorController = null;

        enemyDialogueTransform = enemy.transform.Find("DialogueBubbleHolder").transform;
        speakers = new List<Transform>() { enemyDialogueTransform , playerDialogueTransform };

        #region Death Scene - Detransformation

        groundMonk.GetComponent<EnemyMover>().Flip(Vector2.right);

        playableDirector.playableAsset = timelines[2];

        Bind(playableDirector, "Monk Animations", enemy);

        playableDirector.extrapolationMode = DirectorWrapMode.Hold;
        playableDirector.Play();

        yield return new WaitForSeconds(2f);

        GameManager.instance.GetDialogueManager().RunScreenOverlayDialogue(dialogues[2], 0, 0);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[2], new List<Transform>() { enemyDialogueTransform, playerDialogueTransform }, 1, 1);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        GameManager.instance.GetDialogueManager().RunScreenOverlayDialogue(dialogues[2], 2, 2);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        if (!GameManager.instance.GetDialogueManager().IsRunning && playableDirector.state == PlayState.Playing)
            playableDirector.Stop();

        #endregion

        #region Death Scene - Death

        playableDirector.extrapolationMode = DirectorWrapMode.None;

        playableDirector.playableAsset = timelines[3];

        Bind(playableDirector, "Monk Animations", enemy);
        playableDirector.Play();

        #endregion

        player.GetComponent<DamageReceiver>().enabled = true;
        ActivatePlayer(player);

        StartCoroutine(GameManager.instance.GetLevelLoader().CinematicBracketsEnd());
    }
}
