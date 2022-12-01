using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstFightingRouteTransition : MonoBehaviour
{
    private BoxCollider transitionTrigger;

    [SerializeField] private GameObject cutscene;
    [SerializeField] private GameObject groundMonk;

    private GameObject player;

    [SerializeField] private bool isActivated = false;
    [SerializeField] private bool hasDied = false;

    private void Awake()
    {
        transitionTrigger = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        player = GameManager.instance.GetPlayer();
        StartCoroutine(SetGroundMonk(Config.MEDIUM_DELAY));

        FrameManager frameManager = GameObject.FindGameObjectWithTag(Config.FRAME_MANAGER_TAG).GetComponent<FrameManager>();
        frameManager.OnFrameRestart += FrameRestarted;
    }

    private void OnEnable()
    {
        player = GameManager.instance.GetPlayer();

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
        if (!player.GetComponent<DamageReceiver>().IsAlive)
        {
            if (!hasDied)
                hasDied = true;
        }
        else
        {
            isActivated = false;

            StartCoroutine(SetGroundMonk(1f));
        }
    }

    private void FixedUpdate()
    {
        if (!isActivated && transitionTrigger.IsColliding())
        {
            isActivated = true;

            if (!GameManager.instance.GetCurrentProgressManager().FinishedRoute1 && !hasDied)
            {
                cutscene.SetActive(true);
                cutscene.GetComponent<Cutscene04>().PlayWelcomeCutscene();
            }
            else
            {
                StartCoroutine(ActivateGroundMonk());
            }
        }
    }

    private IEnumerator ActivateGroundMonk()
    {

        groundMonk.GetComponent<Animator>().SetTrigger("Fight");

        yield return null;

        groundMonk.GetComponent<GroundMonk>().enabled = true;
    }

    private IEnumerator SetGroundMonk(float delay)
    {
        yield return new WaitForSeconds(delay);

        groundMonk = GameObject.FindGameObjectWithTag(Config.ENEMY_TAG);

        while (groundMonk == null)
        {
            groundMonk = GameObject.FindGameObjectWithTag(Config.ENEMY_TAG);

            yield return null;
        }
    }

    private void FrameRestarted()
    {
        isActivated = false;
        groundMonk = null;

        StartCoroutine(SetGroundMonk(1.7f));
    }
}
