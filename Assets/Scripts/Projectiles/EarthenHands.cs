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

        if (followingTime <= 0)
        {
            isFollowing = false;

            if (Mathf.Abs(lookat.position.x - transform.position.x) < .2f)
            {
                CatchPlayer();
            }
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
        GameManager.instance.GetPlayer().GetComponent<PlayerMovementController>().BlockMovement(true);
    }

    private IEnumerator WaitAndDestroy(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);

        GameManager.instance.GetPlayer().GetComponent<PlayerMovementController>().BlockMovement(false);
        Destroy(gameObject);
    }

    public void SetLookat(Transform lookat)
    {
        this.lookat = lookat;
    }
}
