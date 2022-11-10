using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Cutscene04 : MonoBehaviour
{
    private Transform enemyDialogueTransform;

    [SerializeField] private List<DialogueObject> dialogues;

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
        yield return new WaitForSeconds(Config.MEDIUM_DELAY);

        GameObject player = GameManager.instance.GetPlayer();

        player.GetComponent<PlayerAttackController>().enabled = false;
        player.GetComponent<PlayerMovementController>().enabled = false;

        StartCoroutine(WaitUntilGroundedToFreezePlayer(player));

        enemy = GameObject.FindGameObjectWithTag("Enemy");
        enemyDialogueTransform = enemy.transform.Find("DialogueBubbleHolder").transform;
        speakers = new List<Transform>() { enemyDialogueTransform };

        playableDirector.playableAsset = timelines[0];

        Bind(playableDirector, "Monk Animations", enemy.GetComponent<Animator>());
        Bind(playableDirector, "Akate Animations", GameManager.instance.GetPlayer().GetComponent<Animator>());

        playableDirector.Play();

        yield return new WaitForSeconds(2f);

        // TODO: change later so it plays in the middle of the screen instead, just
        // like deaths
        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[0], speakers);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        if (!GameManager.instance.GetDialogueManager().IsRunning && playableDirector.state == PlayState.Playing)
            playableDirector.Stop();

        player.GetComponent<PlayerAttackController>().enabled = true;
        player.GetComponent<PlayerMovementController>().enabled = true;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        yield return new WaitForSeconds(Config.SMALL_DELAY);

        enemy.GetComponent<Animator>().SetTrigger("Fight");
        enemy.GetComponent<GroundMonk>().enabled = true;
    }

    public IEnumerator PlayTransformScene()
    {
        yield return new WaitForSeconds(Config.LARGE_DELAY);

        GameObject player = GameManager.instance.GetPlayer();

        player.GetComponent<PlayerAttackController>().enabled = false;
        player.GetComponent<PlayerMovementController>().enabled = false;

        StartCoroutine(WaitUntilGroundedToFreezePlayer(player));

        player.GetComponent<DamageReceiver>().enabled = false;

        enemy = GameObject.FindGameObjectWithTag("Enemy");
        GroundMonk groundMonk = enemy.GetComponent<GroundMonk>();
        groundMonk.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        enemyDialogueTransform = enemy.transform.Find("DialogueBubbleHolder").transform;
        speakers = new List<Transform>() { enemyDialogueTransform };

        playableDirector.playableAsset = timelines[1];

        Bind(playableDirector, "Monk Animations", enemy.GetComponent<Animator>());
        Bind(playableDirector, "Akate Animations", GameManager.instance.GetPlayer().GetComponent<Animator>());

        playableDirector.extrapolationMode = DirectorWrapMode.Loop;
        playableDirector.Play();

        yield return new WaitForSeconds(1f);

        // TODO: change later so it plays in the middle of the screen instead, just
        // like deaths
        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[1], speakers);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        if (!GameManager.instance.GetDialogueManager().IsRunning && playableDirector.state == PlayState.Playing)
            playableDirector.Stop();

        playableDirector.extrapolationMode = DirectorWrapMode.None;

        GameManager.instance.GetCurrentProgressManager().Route1BossHasTransformed = true;

        yield return null;

        groundMonk.Transform();

        player.GetComponent<PlayerAttackController>().enabled = true;
        player.GetComponent<PlayerMovementController>().enabled = true;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        player.GetComponent<DamageReceiver>().enabled = true;

        yield return new WaitForSeconds(3.7f);

        groundMonk.enabled = true;
        groundMonk.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }

    public IEnumerator PlayDeathScene()
    {
        GameObject player = GameManager.instance.GetPlayer();

        player.GetComponent<PlayerAttackController>().enabled = false;
        player.GetComponent<PlayerMovementController>().enabled = false;

        StartCoroutine(WaitUntilGroundedToFreezePlayer(player));

        Transform playerDialogueTransform = player.transform.Find("DialogueBubble Transform").transform;

        enemy = GameObject.FindGameObjectWithTag("Enemy");
        GroundMonk groundMonk = enemy.GetComponent<GroundMonk>();
        groundMonk.enabled = false;
        groundMonk.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        groundMonk.GetComponent<Animator>().runtimeAnimatorController = null;

        enemyDialogueTransform = enemy.transform.Find("DialogueBubbleHolder").transform;
        speakers = new List<Transform>() { enemyDialogueTransform };

        playableDirector.playableAsset = timelines[1];

        Bind(playableDirector, "Monk Animations", enemy.GetComponent<Animator>());
        Bind(playableDirector, "Akate Animations", GameManager.instance.GetPlayer().GetComponent<Animator>());

        playableDirector.extrapolationMode = DirectorWrapMode.Hold;
        playableDirector.Play();

        yield return new WaitForSeconds(1f);

        // TODO: change later so it plays in the middle of the screen instead, just
        // like deaths
        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[2], speakers, 0, 0);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }
        
        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[2], new List<Transform>() { enemyDialogueTransform, playerDialogueTransform }, 1, 1);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        // TODO: change later so it plays in the middle of the screen instead, just
        // like deaths
        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[2], speakers, 2, 2);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        if (!GameManager.instance.GetDialogueManager().IsRunning && playableDirector.state == PlayState.Playing)
            playableDirector.Stop();

        playableDirector.extrapolationMode = DirectorWrapMode.None;

        playableDirector.playableAsset = timelines[2];

        Bind(playableDirector, "Monk Animations", enemy.GetComponent<Animator>());
        Bind(playableDirector, "Akate Animations", GameManager.instance.GetPlayer().GetComponent<Animator>());
        playableDirector.Play();

        player.GetComponent<PlayerAttackController>().enabled = true;
        player.GetComponent<PlayerMovementController>().enabled = true;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
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

    private IEnumerator WaitUntilGroundedToFreezePlayer(GameObject player)
    {
        while(!player.GetComponent<Mover>().IsGrounded())
        {
            yield return null;
        }

        player.GetComponent<PlayerMovementController>().StayInPosition();
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
