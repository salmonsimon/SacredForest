using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameManager : MonoBehaviour
{
    [SerializeField] private Frame[] frames;

    private Frame activeFrame;
    private int activeFrameIndex = 0;

    private GameObject player;

    private void Start()
    {
        player = GameManager.instance.GetPlayer();

        activeFrame = frames[activeFrameIndex];

        activeFrame.gameObject.SetActive(true);
        activeFrame.StartFrame();
    }

    private void Update()
    {
        if (!player.GetComponent<DamageReceiver>().IsAlive && !GameManager.instance.IsTeleporting())
        {
            GameManager.instance.GetAnimationManager().ShowImageUI("Space Key", true);

            if (Input.GetKeyDown(KeyCode.Space) && !GameManager.instance.IsTeleporting())
            {
                GameManager.instance.GetAnimationManager().ShowImageUI("Space Key", false);
                activeFrame.ShowArrowUI(false);
                StartCoroutine(RestartFrame());
            }
        }
    }

    private void FixedUpdate()
    {
        if (activeFrame.FrameChangeTriggered() && !GameManager.instance.IsTeleporting())
        {
            activeFrame.ShowArrowUI(false);
            StartCoroutine(NextFrame());
        }
    }

    private IEnumerator NextFrame()
    {
        GameManager.instance.SetIsTeleporting(true);

        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GameManager.instance.GetPlayer().GetComponent<Animator>().enabled = false;
        GameManager.instance.GetPlayer().GetComponent<PlayerAttackController>().EndAttackCooldown();

        GameManager.instance.GetLevelLoader().CrossfadeStart();
        yield return new WaitForSeconds(Config.START_TRANSITION_DURATION);

        activeFrameIndex++;

        activeFrame.CleanFrame();
        activeFrame.gameObject.SetActive(false);

        activeFrame = frames[activeFrameIndex];

        StartCoroutine(GameManager.instance.GetLevelLoader().CrossfadeEnd());

        activeFrame.gameObject.SetActive(true);
        activeFrame.StartFrame();

        yield return new WaitForSeconds(Config.END_TRANSITION_DURATION);

        GameManager.instance.SetIsTeleporting(false);

        GameManager.instance.GetPlayer().GetComponent<Animator>().enabled = true;
        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private IEnumerator RestartFrame()
    {
        GameManager.instance.SetIsTeleporting(true);
        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GameManager.instance.GetPlayer().GetComponent<PlayerAttackController>().EndAttackCooldown();

        GameManager.instance.GetLevelLoader().CrossfadeStart();

        yield return new WaitForSeconds(Config.START_TRANSITION_DURATION);

        StartCoroutine(GameManager.instance.GetLevelLoader().CrossfadeEnd());

        activeFrame.RestartFrame();

        yield return new WaitForSeconds(Config.END_TRANSITION_DURATION);

        GameManager.instance.SetIsTeleporting(false);
        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
