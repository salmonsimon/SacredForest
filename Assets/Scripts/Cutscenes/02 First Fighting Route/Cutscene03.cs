using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Cutscene03 : Cutscene
{
    private Transform enemyDialogueTransform;
    private GameObject enemy;

    protected override void Start()
    {
        base.Start();


        if (!GameManager.instance.GetCurrentProgressManager().FinishedRoute1)
        {
            StartCoroutine(Play());

            FrameManager frameManager = GameObject.FindGameObjectWithTag(Config.FRAME_MANAGER_TAG).GetComponent<FrameManager>();
            frameManager.OnFrameRestart += FrameRestarted;
        }
            
    }

    private void OnDisable()
    {
        FrameManager frameManager = GameObject.FindGameObjectWithTag(Config.FRAME_MANAGER_TAG).GetComponent<FrameManager>();
        frameManager.OnFrameRestart -= FrameRestarted;
    }

    private IEnumerator Play()
    {
        GameManager.instance.GetLevelLoader().CinematicBracketsStart();
        GameManager.instance.GetSFXManager().PlaySound(Config.SUSPENSE_SFX);

        GameObject player = GameManager.instance.GetPlayer();
        DeactivatePlayer();

        yield return new WaitForSeconds(Config.MEDIUM_DELAY);

        enemy = GameObject.FindGameObjectWithTag("Enemy");

        while (enemy == null)
        {
            enemy = GameObject.FindGameObjectWithTag("Enemy");
            yield return null;
        }

        enemy.GetComponent<Swordsman>().enabled = false;

        enemyDialogueTransform = enemy.transform.Find("DialogueBubbleHolder").transform;
        speakers = new List<Transform>() { enemyDialogueTransform };

        #region First Mini Boss

        playableDirector.playableAsset = timelines[0];

        Bind(playableDirector, "Knight Animations", enemy);
        Bind(playableDirector, "Akate Animations", player);

        playableDirector.Play();

        yield return new WaitForSeconds(1f + Config.BIG_DELAY + Config.MEDIUM_DELAY);

        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[0], speakers);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        if (!GameManager.instance.GetDialogueManager().IsRunning && playableDirector.state == PlayState.Playing)
            playableDirector.Stop();

        #endregion

        StartCoroutine(GameManager.instance.GetLevelLoader().CinematicBracketsEnd());

        yield return new WaitForSeconds(1f);

        ActivatePlayer();

        enemy.GetComponent<Swordsman>().enabled = true;
        enemy.GetComponent<Swordsman>().SetStartleDuration(Config.MEDIUM_DELAY);
        
        this.enabled = false;
    }

    protected override void FrameRestarted()
    {
        base.FrameRestarted();

        StartCoroutine(WaitAndPlayCutscene());
    }

    private IEnumerator WaitAndPlayCutscene()
    {
        yield return new WaitForSeconds(1.7f);

        StartCoroutine(Play());
    }
}
