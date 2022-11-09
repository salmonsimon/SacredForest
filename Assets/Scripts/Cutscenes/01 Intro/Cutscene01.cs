using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Cutscene01 : MonoBehaviour
{
    private Transform foxyDialogueTransform;
    private Transform akateDialogueTransform;

    [SerializeField] private DialogueObject dialogue;

    private PlayableDirector playableDirector;

    [SerializeField] private List<TimelineAsset> timelines;

    [SerializeField] private List<Transform> speakers = new List<Transform>();

    private void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();

        foxyDialogueTransform = GameObject.Find("Foxy/DialogueBubble Transform").transform;
        akateDialogueTransform = GameManager.instance.GetPlayer().transform.Find("DialogueBubble Transform").transform;
        speakers = new List<Transform>() { foxyDialogueTransform, akateDialogueTransform };

        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        GameManager.instance.GetPlayer().GetComponent<PlayerAttackController>().enabled = false;
        GameManager.instance.GetPlayer().GetComponent<PlayerMovementController>().enabled = false;
        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        RuntimeAnimatorController playerAnimator = GameManager.instance.GetPlayer().GetComponent<Animator>().runtimeAnimatorController;
        GameManager.instance.GetPlayer().GetComponent<Animator>().runtimeAnimatorController = null;

        GameObject foxy = GameObject.FindWithTag("Foxy");

        playableDirector.playableAsset = timelines[0];

        Bind(playableDirector, "Foxy Animations", foxy.GetComponent<Animator>());
        Bind(playableDirector, "Akate Animations", GameManager.instance.GetPlayer().GetComponent<Animator>());
        Bind(playableDirector, "Crossfade Animation", GameManager.instance.GetLevelLoader().GetCrossfadeAnimator());

        playableDirector.extrapolationMode = DirectorWrapMode.Loop;
        playableDirector.Play();

        yield return new WaitForSeconds(3f);
        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogue, speakers, 0, 3);

        
        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }
        
        if (!GameManager.instance.GetDialogueManager().IsRunning && playableDirector.state == PlayState.Playing)
            playableDirector.Stop();

        yield return new WaitForSeconds(Config.MEDIUM_DELAY);

        playableDirector.playableAsset = timelines[1];

        Bind(playableDirector, "Foxy Animations", foxy.GetComponent<Animator>());
        Bind(playableDirector, "Foxy Movements", foxy.GetComponent<Animator>());
        Bind(playableDirector, "Akate Animations", GameManager.instance.GetPlayer().GetComponent<Animator>());

        playableDirector.extrapolationMode = DirectorWrapMode.None;
        playableDirector.Play();

        yield return new WaitForSeconds(1f);

        GameManager.instance.GetPlayer().GetComponent<Animator>().runtimeAnimatorController = playerAnimator;

        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogue, speakers, 4);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        yield return new WaitForSeconds(Config.SMALL_DELAY);

        playableDirector.playableAsset = timelines[2];

        Bind(playableDirector, "Foxy Animations", foxy.GetComponent<Animator>());
        Bind(playableDirector, "Foxy Movements", foxy.GetComponent<Animator>());

        foxy.GetComponent<SpriteRenderer>().flipX = false;

        playableDirector.Play();

        yield return new WaitForSeconds(1f);

        foxy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        foxy.GetComponent<FoxyController>().enabled = true;

        GameManager.instance.GetPlayer().GetComponent<PlayerAttackController>().enabled = true;
        GameManager.instance.GetPlayer().GetComponent<PlayerMovementController>().enabled = true;
        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

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
