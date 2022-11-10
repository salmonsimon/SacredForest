using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstFightingRouteTransition : MonoBehaviour
{
    private BoxCollider transitionTrigger;

    [SerializeField] private GameObject cutscene;
    private GameObject groundMonk;

    private GameObject player;

    private bool isActivated = false;
    private bool hasDied = false;

    private void Awake()
    {
        transitionTrigger = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        player = GameManager.instance.GetPlayer();
        StartCoroutine(SetGroundMonk(Config.MEDIUM_DELAY));
    }

    private void Update()
    {
        if (!player.GetComponent<DamageReceiver>().IsAlive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isActivated = false;
                hasDied = true;

                StartCoroutine(SetGroundMonk(1f));
            }
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
            }
            else
            {
                StartCoroutine(ActivateGroundMonk());
            }
        }
    }

    private IEnumerator ActivateGroundMonk()
    {
        yield return new WaitForSeconds(Config.MEDIUM_DELAY);

        groundMonk.GetComponent<Animator>().SetTrigger("Fight");

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        groundMonk.GetComponent<GroundMonk>().enabled = true;
    }

    private IEnumerator SetGroundMonk(float delay)
    {
        yield return new WaitForSeconds(delay);

        groundMonk = GameObject.FindGameObjectWithTag(Config.ENEMY_TAG);
    }
}
