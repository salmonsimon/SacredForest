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

    private GameObject player;

    private void Awake()
    {
        transitionTrigger = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        player = GameManager.instance.GetPlayer();
    }

    private void Update()
    {
        if (!player.GetComponent<DamageReceiver>().IsAlive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                activeVcam.Priority = 1;
                otherVcam.Priority = 0;

                blockCollider.gameObject.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (transitionTrigger.IsColliding())
        {
            activeVcam.Priority = 0;
            otherVcam.Priority = 1;

            blockCollider.gameObject.SetActive(true);
        }
    }
}
