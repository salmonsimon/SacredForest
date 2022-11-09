using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Cutscene02 : MonoBehaviour
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
        foxy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        Destroy(foxy.GetComponent<Rigidbody2D>());
        foxy.GetComponent<FoxyController>().StayInPosition();
        foxy.GetComponent<FoxyController>().enabled = false;

        playableDirector.playableAsset = timelines[0];

        Bind(playableDirector, "Foxy Animations", foxy.GetComponent<Animator>());
        Bind(playableDirector, "Foxy Movement", foxy.GetComponent<Animator>());
        Bind(playableDirector, "Akate Animations", GameManager.instance.GetPlayer().GetComponent<Animator>());
        Bind(playableDirector, "Akate Movement", GameManager.instance.GetPlayer().GetComponent<Animator>());
        Bind(playableDirector, "Crossfade Animation", GameManager.instance.GetLevelLoader().GetCrossfadeAnimator());

        playableDirector.Play();

        yield return new WaitForSeconds(1f);

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

        foxy.GetComponent<InteractibleNPC>().enabled = true;
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
