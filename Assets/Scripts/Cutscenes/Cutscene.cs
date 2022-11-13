using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Cutscene : MonoBehaviour
{
    protected PlayableDirector playableDirector;

    protected List<Transform> speakers = new List<Transform>();

    [SerializeField] protected List<DialogueObject> dialogues;

    [SerializeField] protected List<TimelineAsset> timelines;

    protected virtual void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }

    protected static void Bind(PlayableDirector director, string trackName, GameObject gameObject)
    {
        var timeline = director.playableAsset as TimelineAsset;
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track.name == trackName)
            {
                director.SetGenericBinding(track, gameObject);
                break;
            }
        }
    }

    protected IEnumerator WaitUntilGroundedToFreezePlayer(GameObject player)
    {
        while (!player.GetComponent<Mover>().IsGrounded())
        {
            yield return null;
        }

        player.GetComponent<PlayerMovementController>().StayInPosition();
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    protected void DeactivatePlayer(GameObject player)
    {
        player.GetComponent<PlayerAttackController>().enabled = false;
        player.GetComponent<PlayerMovementController>().enabled = false;

        StartCoroutine(WaitUntilGroundedToFreezePlayer(player));
    }

    protected void ActivatePlayer(GameObject player)
    {
        GameManager.instance.GetPlayer().GetComponent<PlayerAttackController>().enabled = true;
        GameManager.instance.GetPlayer().GetComponent<PlayerMovementController>().enabled = true;
        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
