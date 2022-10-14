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
        if (!player.GetComponent<DamageReceiver>().IsAlive)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !GameManager.instance.IsTeleporting())
            {
                StartCoroutine(RestartFrame());
            }
        }
    }

    private void FixedUpdate()
    {
        if (activeFrame.FrameChangeTriggered() && !GameManager.instance.IsTeleporting())
        {
            StartCoroutine(NextFrame());
        }
    }

    private IEnumerator NextFrame()
    {
        GameManager.instance.SetIsTeleporting(true);

        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GameManager.instance.GetPlayer().GetComponent<Animator>().enabled = false;

        GameManager.instance.GetLevelLoader().CrossfadeStart();
        yield return new WaitForSeconds(Config.START_TRANSITION_DURATION);

        activeFrameIndex++;

        activeFrame.CleanFrame();
        activeFrame.gameObject.SetActive(false);

        activeFrame = frames[activeFrameIndex];

        activeFrame.gameObject.SetActive(true);
        activeFrame.StartFrame();

        GameManager.instance.SetIsTeleporting(false);

        StartCoroutine(GameManager.instance.GetLevelLoader().CrossfadeEnd());
        yield return new WaitForSeconds(Config.END_TRANSITION_DURATION);

        GameManager.instance.GetPlayer().GetComponent<Animator>().enabled = true;
        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private IEnumerator RestartFrame()
    {
        GameManager.instance.SetIsTeleporting(true);

        GameManager.instance.GetLevelLoader().CrossfadeStart();

        yield return new WaitForSeconds(Config.START_TRANSITION_DURATION);

        activeFrame.RestartFrame();

        GameManager.instance.SetIsTeleporting(false);

        StartCoroutine(GameManager.instance.GetLevelLoader().CrossfadeEnd());
    }
}
