using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameManager : MonoBehaviour
{
    [SerializeField] private Frame[] frames;

    private Frame activeFrame;
    private int activeFrameIndex = 0;

    [SerializeField] private GameObject player;
    [SerializeField] private DamageReceiver playerDamageReceiver;

    public delegate void OnFrameRestartDelegate();
    public event OnFrameRestartDelegate OnFrameRestart;

    private void Start()
    {
        player = GameManager.instance.GetPlayer();
        playerDamageReceiver = player.GetComponent<DamageReceiver>();

        activeFrame = frames[activeFrameIndex];

        activeFrame.gameObject.SetActive(true);
        activeFrame.StartFrame();
    }

    private void Update()
    {
        if (!player || !playerDamageReceiver)
        {
            player = GameManager.instance.GetPlayer();
            playerDamageReceiver = player.GetComponent<DamageReceiver>();
        }

        if (!playerDamageReceiver.IsAlive && !GameManager.instance.IsTeleporting() && !GameManager.instance.GetDialogueManager().IsRunning)
        {
            GameManager.instance.GetAnimationManager().ShowImageUI(Config.SPACE_KEY_GUI, true);

            if (Input.GetKeyDown(KeyCode.Space) && !GameManager.instance.IsTeleporting())
            {
                GameManager.instance.GetAnimationManager().ShowImageUI(Config.SPACE_KEY_GUI, false);
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

        GameManager.instance.GetLevelLoader().CrossfadeStart();
        yield return new WaitForSeconds(Config.START_TRANSITION_DURATION * 2);

        GameManager.instance.GetPlayer().GetComponent<Animator>().enabled = true;
        GameManager.instance.RestartPlayer();

        activeFrameIndex++;

        activeFrame.CleanFrame();
        activeFrame.gameObject.SetActive(false);

        activeFrame = frames[activeFrameIndex];

        StartCoroutine(GameManager.instance.GetLevelLoader().CrossfadeEnd());

        activeFrame.gameObject.SetActive(true);
        activeFrame.StartFrame();

        GameObject[] parallaxBackgrounds = GameObject.FindGameObjectsWithTag(Config.PARALLAX_BACKGROUND_TAG);
        foreach (GameObject parallaxBackground in parallaxBackgrounds)
        {
            parallaxBackground.GetComponent<ParallaxBackground>().SetYLimit();
        }

        yield return new WaitForSeconds(Config.END_TRANSITION_DURATION);

        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public IEnumerator RestartFrame()
    {
        if (OnFrameRestart != null)
            OnFrameRestart();

        GameManager.instance.SetIsTeleporting(true);
        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        GameManager.instance.GetLevelLoader().CrossfadeStart();

        yield return new WaitForSeconds(Config.START_TRANSITION_DURATION);

        StartCoroutine(GameManager.instance.GetLevelLoader().CrossfadeEnd());

        activeFrame.RestartFrame();

        yield return new WaitForSeconds(Config.END_TRANSITION_DURATION + Config.MEDIUM_DELAY);

        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
