using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Cutscene01 : MonoBehaviour
{
    [SerializeField] private Transform foxyDialogueTransform;
    [SerializeField] private Transform akateDialogueTransform;

    [SerializeField] private DialogueObject dialogue;

    private PlayableDirector playableDirector;

    [SerializeField] private List<TimelineAsset> timelines;

    [SerializeField] private List<Transform> speakers = new List<Transform>();

    private void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();

        foxyDialogueTransform = GameObject.Find("Foxy/DialogueBubble Transform").transform;
        akateDialogueTransform = GameManager.instance.GetPlayer().transform.Find("DialogueBubble Transform").transform;

        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        // TODO: DELETE THIS AFTERWARDS
        yield return new WaitForSeconds(2f);

        GameManager.instance.GetPlayer().GetComponent<PlayerAttackController>().enabled = false;
        GameManager.instance.GetPlayer().GetComponent<PlayerMovementController>().enabled = false;
        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        RuntimeAnimatorController playerAnimator = GameManager.instance.GetPlayer().GetComponent<Animator>().runtimeAnimatorController;
        GameManager.instance.GetPlayer().GetComponent<Animator>().runtimeAnimatorController = null;

        speakers = new List<Transform>() { foxyDialogueTransform, akateDialogueTransform };

        playableDirector.playableAsset = timelines[0];

        Bind(playableDirector, "Foxy Animations", GameObject.FindWithTag("Foxy").GetComponent<Animator>());
        Bind(playableDirector, "Akate Animations", GameManager.instance.GetPlayer().GetComponent<Animator>());
        Bind(playableDirector, "Crossfade Animation", GameManager.instance.GetLevelLoader().GetCrossfadeAnimator());

        playableDirector.Play();

        GameObject player = GameManager.instance.GetPlayer();

        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogue, speakers, 0, 3);

        /*
        GameManager.instance.GetDialogueManager().RunDialogue(dialogue, speakers,0, 3);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }
        

        if (!GameManager.instance.GetDialogueManager().IsRunning && playableDirector.state == PlayState.Playing)
            playableDirector.Stop();
        

        playableDirector.Play(timelines[1]);

        yield return new WaitForSeconds(1f);

        GameManager.instance.GetDialogueManager().RunDialogue(dialogue, speakers, 4);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        GameManager.instance.GetPlayer().GetComponent<PlayerAttackController>().enabled = true;
        GameManager.instance.GetPlayer().GetComponent<PlayerMovementController>().enabled = true;
        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        GameManager.instance.GetPlayer().GetComponent<Animator>().runtimeAnimatorController = playerAnimator;
        
         */
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
