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

    private List<Transform> speakers = new List<Transform>();

    [SerializeField] private GameObject foxyNPC;

    private void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();

        foxyDialogueTransform = GameObject.Find("Foxy - Intro/DialogueBubble Transform").transform;
        akateDialogueTransform = GameManager.instance.GetPlayer().transform.Find("DialogueBubble Transform").transform;
        speakers = new List<Transform>() { foxyDialogueTransform, akateDialogueTransform };

        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        GameObject player = GameManager.instance.GetPlayer();

        player.GetComponent<PlayerAttackController>().enabled = false;
        player.GetComponent<PlayerMovementController>().enabled = false;

        StartCoroutine(WaitUntilGroundedToFreezePlayer(player));

        GameObject foxy = GameObject.FindWithTag("Foxy");
        foxy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        foxy.GetComponent<FoxyController>().StayInPosition();
        foxy.GetComponent<FoxyController>().enabled = false;

        playableDirector.playableAsset = timelines[0];

        Bind(playableDirector, "Foxy Animations", foxy.GetComponent<Animator>());
        Bind(playableDirector, "Akate Animations", GameManager.instance.GetPlayer().GetComponent<Animator>());

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

        foxy.AddComponent<NPC>();

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

    private IEnumerator WaitUntilGroundedToFreezePlayer(GameObject player)
    {
        while (!player.GetComponent<Mover>().IsGrounded())
        {
            yield return null;
        }

        player.GetComponent<PlayerMovementController>().StayInPosition();
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
