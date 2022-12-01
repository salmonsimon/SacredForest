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
    private DamageReceiver playerDamageReceiver;
    private bool isPlayerAlive = true;

    private void Awake()
    {
        transitionTrigger = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        player = GameManager.instance.GetPlayer();
        playerDamageReceiver = player.GetComponent<DamageReceiver>();

        FrameManager frameManager = GameObject.FindGameObjectWithTag(Config.FRAME_MANAGER_TAG).GetComponent<FrameManager>();
        frameManager.OnFrameRestart += FrameRestarted;
    }

    private void OnEnable()
    {
        player = GameManager.instance.GetPlayer();
        playerDamageReceiver = player.GetComponent<DamageReceiver>();

        player.GetComponent<DamageReceiver>().OnCharacterAliveStatusChange += PlayerAliveStatusChange;
    }
    private void OnDisable()
    {
        if (player == null)
            player = GameManager.instance.GetPlayer();

        player.GetComponent<DamageReceiver>().OnCharacterAliveStatusChange -= PlayerAliveStatusChange;

        FrameManager frameManager = GameObject.FindGameObjectWithTag(Config.FRAME_MANAGER_TAG).GetComponent<FrameManager>();
        frameManager.OnFrameRestart -= FrameRestarted;
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
        CheckIfPlayerAlive();

        if (transitionTrigger.IsColliding())
        {
            activeVcam.Priority = 0;
            otherVcam.Priority = 1;

            GameManager.instance.GetCinemachineShake().SetVirtualCamera();

            blockCollider.gameObject.SetActive(true);

            if (GameManager.instance.GetMusicManager().GetCurrentAudioClipName() != "First Boss - Comigo")
            {
                GameManager.instance.GetSFXManager().PlaySound(Config.SUSPENSE_SFX);

                GameManager.instance.GetMusicManager().StopMusic();
                GameManager.instance.GetMusicManager().PlayMusic(Config.FIRST_BOSS_FRAME);
            }
        }
    }

    private void CheckIfPlayerAlive()
    {
        if (playerDamageReceiver.IsAlive && !isPlayerAlive)
        {
            if (activeVcam.Priority < otherVcam.Priority)
                PlayerAliveStatusChange();
        }
        else if (!playerDamageReceiver.IsAlive && isPlayerAlive)
            isPlayerAlive = false;
    }

    private void FrameRestarted()
    {
        activeVcam.Priority = 1;
        otherVcam.Priority = 0;

        GameManager.instance.GetCinemachineShake().SetVirtualCamera();

        blockCollider.gameObject.SetActive(false);
        isPlayerAlive = true;
    }
}
