using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Cutscene03 : MonoBehaviour
{
    private Transform enemyDialogueTransform;

    [SerializeField] private DialogueObject dialogue;

    private PlayableDirector playableDirector;

    [SerializeField] private List<TimelineAsset> timelines;

    private List<Transform> speakers = new List<Transform>();
    private GameObject enemy;

    private void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();

        if (!ProgressManager.Instance.finishedRoute1)
            StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        GameManager.instance.GetPlayer().GetComponent<PlayerAttackController>().enabled = false;
        GameManager.instance.GetPlayer().GetComponent<PlayerMovementController>().enabled = false;
        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        RuntimeAnimatorController playerAnimator = GameManager.instance.GetPlayer().GetComponent<Animator>().runtimeAnimatorController;
        GameManager.instance.GetPlayer().GetComponent<Animator>().runtimeAnimatorController = null;

        yield return new WaitForSeconds(Config.MEDIUM_DELAY);

        enemy = GameObject.FindGameObjectWithTag("Enemy");
        enemy.GetComponent<Swordsman>().enabled = false;
        enemyDialogueTransform = enemy.transform.Find("DialogueBubbleHolder").transform;
        speakers = new List<Transform>() { enemyDialogueTransform };

        playableDirector.playableAsset = timelines[0];

        Bind(playableDirector, "Knight Animations", enemy.GetComponent<Animator>());
        Bind(playableDirector, "Akate Animations", GameManager.instance.GetPlayer().GetComponent<Animator>());

        playableDirector.Play();

        yield return new WaitForSeconds(Config.BIG_DELAY);

        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogue, speakers);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        if (!GameManager.instance.GetDialogueManager().IsRunning && playableDirector.state == PlayState.Playing)
            playableDirector.Stop();

        GameManager.instance.GetPlayer().GetComponent<PlayerAttackController>().enabled = true;
        GameManager.instance.GetPlayer().GetComponent<PlayerMovementController>().enabled = true;
        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        GameManager.instance.GetPlayer().GetComponent<Animator>().runtimeAnimatorController = playerAnimator;

        enemy.GetComponent<Swordsman>().enabled = true;
        enemy.GetComponent<Swordsman>().SetStartleDuration(Config.MEDIUM_DELAY);
        this.enabled = false;
    }

    public static void Bind(PlayableDirector director, string trackName, Animator animator)
    {
        var timeline = director.playableAsset as TimelineAsset;
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track.name == trackName)
            {
                director.SetGenericBinding(track, animator);
                break;
            }
        }
    }
}
