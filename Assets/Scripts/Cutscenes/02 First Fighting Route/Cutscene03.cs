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
            StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        GameManager.instance.GetLevelLoader().CinematicBracketsStart();

        GameObject player = GameManager.instance.GetPlayer();
        DeactivatePlayer(player);

        yield return new WaitForSeconds(Config.MEDIUM_DELAY);

        enemy = GameObject.FindGameObjectWithTag("Enemy");
        enemy.GetComponent<Swordsman>().enabled = false;

        enemyDialogueTransform = enemy.transform.Find("DialogueBubbleHolder").transform;
        speakers = new List<Transform>() { enemyDialogueTransform };

        #region First Mini Boss

        playableDirector.playableAsset = timelines[0];

        Bind(playableDirector, "Knight Animations", enemy);
        Bind(playableDirector, "Akate Animations", player);

        playableDirector.Play();

        yield return new WaitForSeconds(Config.BIG_DELAY);

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

        ActivatePlayer(player);

        enemy.GetComponent<Swordsman>().enabled = true;
        enemy.GetComponent<Swordsman>().SetStartleDuration(Config.MEDIUM_DELAY);
        
        this.enabled = false;
    }
}
