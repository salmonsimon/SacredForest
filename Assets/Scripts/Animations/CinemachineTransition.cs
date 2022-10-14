using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineTransition : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera activeVcam;
    [SerializeField] CinemachineVirtualCamera otherVcam;

    private BoxCollider transitionTrigger;

    [SerializeField] BoxCollider2D blockCollider;

    private void Awake()
    {
        transitionTrigger = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        if (transitionTrigger.IsColliding())
        {
            activeVcam.Priority = 0;
            otherVcam.Priority = 1;

            blockCollider.gameObject.SetActive(true);

            Destroy(gameObject);
        }
    }
}
