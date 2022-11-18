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
    private bool isPlayerAlive = true;

    private void Awake()
    {
        transitionTrigger = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        player = GameManager.instance.GetPlayer();

        player.GetComponent<DamageReceiver>().OnCharacterAliveStatusChange += PlayerAliveStatusChange;
    }

    private void PlayerAliveStatusChange()
    {
        if (!isPlayerAlive)
        {
            activeVcam.Priority = 1;
            otherVcam.Priority = 0;

            GameManager.instance.GetCinemachineShake().SetVirtualCamera();

            blockCollider.gameObject.SetActive(false);
            isPlayerAlive = true;
        }
        else
        {
            isPlayerAlive = false;
        }
    }

    private void FixedUpdate()
    {
        if (transitionTrigger.IsColliding())
        {
            activeVcam.Priority = 0;
            otherVcam.Priority = 1;

            GameManager.instance.GetCinemachineShake().SetVirtualCamera();

            blockCollider.gameObject.SetActive(true);
        }
    }
}
