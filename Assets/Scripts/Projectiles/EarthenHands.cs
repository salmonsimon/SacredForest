using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthenHands : MonoBehaviour
{
    private Transform lookat;
    private Queue<Vector3> lookatPreviousPositions = new Queue<Vector3>();
    private Vector3 lookatLastPosition = Vector3.zero;

    private float followDuration = 1f;
    private float followingTime = 0;

    private bool isFollowing = true;


    private void Start()
    {
        followingTime = followDuration;

        StartCoroutine(WaitAndDestroy(2f));
    }

    private void Update()
    {
        followingTime -= Time.deltaTime;

        if (followingTime <= .5f)
        {
            if (Mathf.Abs(lookat.position.x - transform.position.x) < .2f && lookat.position.y - transform.position.y < .4f)
            {
                CatchPlayer();
            }
        }

        if (followingTime <= 0)
        {
            isFollowing = false;
        }
        else
        {
            if (lookat.position != lookatLastPosition)
            {
                lookatPreviousPositions.Enqueue(lookat.position);
            }
        }
    }

    public void FixedUpdate()
    {
        if (isFollowing && lookatPreviousPositions.Count > 0)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        Vector3 lookatPreviousPosition = lookatPreviousPositions.Dequeue();

        transform.position = new Vector3(lookatPreviousPosition.x, transform.position.y, transform.position.z);
    }

    private void CatchPlayer()
    {
        isFollowing = false;

        StartCoroutine(SmoothPlayerPlacement());

        GameObject player = GameManager.instance.GetPlayer();
        player.GetComponent<PlayerMovementController>().SetIsAbleToMove(false);
    }

    private IEnumerator SmoothPlayerPlacement()
    {
        float duration = .2f;

        float startTime = Time.time;
        float endTime = startTime + duration;

        GameObject player = GameManager.instance.GetPlayer();

        Vector3 playerOriginalPosition = player.transform.position;
        Vector3 playerFinalPosition = new Vector3(transform.position.x, transform.position.y, player.transform.position.z);

        while (Time.time < endTime)
        {
            player.transform.position = Vector3.Lerp(playerOriginalPosition, playerFinalPosition, (Time.time - startTime) / duration);

            yield return null;
        }
    }

    private IEnumerator WaitAndDestroy(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);

        GameObject player = GameManager.instance.GetPlayer();

        if (player.GetComponent<DamageReceiver>().IsAlive)
            player.GetComponent<PlayerMovementController>().SetIsAbleToMove(true);
        else
        {
            player.GetComponent<PlayerMovementController>().BlockMovement(true);
        }

        Destroy(gameObject);
    }

    public void SetLookat(Transform lookat)
    {
        this.lookat = lookat;
    }
}
