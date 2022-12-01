using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    private Animator animator;

    private DamageReceiver playerDamageReceiver;

    [SerializeField] private GameObject healthPanel;

    [SerializeField] private List<GameObject> hearts;
    [SerializeField] private List<GameObject> currentGameHearts;
    [SerializeField] private List<GameObject> currentShownGameHearts;

    private DifficultyLevel currentDifficultyLevel;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Activate()
    {
        gameObject.SetActive(true);

        playerDamageReceiver = GameManager.instance.GetPlayer().GetComponent<DamageReceiver>();

        playerDamageReceiver.OnCharacterDamaged -= Damaged;
        playerDamageReceiver.OnCharacterAliveStatusChange -= PlayerAliveStatusChange;

        playerDamageReceiver.OnCharacterDamaged += Damaged;
        playerDamageReceiver.OnCharacterAliveStatusChange += PlayerAliveStatusChange;

        currentDifficultyLevel = GameManager.instance.GetCurrentProgressManager().CurrentDifficultyLevel;

        switch (currentDifficultyLevel)
        {
            case DifficultyLevel.Normal:

                currentGameHearts = new List<GameObject>(hearts.GetRange(0, 1));
                RestartCurrentShownHearts();

                break;

            case DifficultyLevel.Easy:

                currentGameHearts = new List<GameObject>(hearts.GetRange(0, 2));
                RestartCurrentShownHearts();

                break;

            case DifficultyLevel.Baby:

                currentGameHearts = new List<GameObject>(hearts.GetRange(0, 3));
                RestartCurrentShownHearts();

                break;
        }
    }

    public void Deactivate()
    {
        playerDamageReceiver.OnCharacterDamaged -= Damaged;
        playerDamageReceiver.OnCharacterAliveStatusChange -= PlayerAliveStatusChange;

        gameObject.SetActive(false);
    }

    public void RestartCurrentShownHearts()
    {
        foreach (Transform shownHeart in healthPanel.transform)
        {
            Destroy(shownHeart.gameObject);
        }

        currentShownGameHearts.Clear();

        foreach (GameObject heart in currentGameHearts)
        {
            GameObject newHeart = Instantiate(heart, healthPanel.transform);

            currentShownGameHearts.Add(newHeart);
        }

        StartCurrentHeartAnimation();
    }

    public void ShowHealth(bool value)
    {
        if (value)
        {
            animator.SetTrigger(Config.ANIMATOR_SHOW_COUNTERS);
        }
        else
        {
            animator.SetTrigger(Config.ANIMATOR_HIDE_COUNTERS);
        }
    }

    public void StartHeartAnimation(int index)
    {
        currentShownGameHearts[index].GetComponent<Animator>().SetTrigger(Config.ANIMATOR_START_CURRENT_HEART);
    }

    public void StartCurrentHeartAnimation()
    {
        StartCoroutine(StartCurrentHeartAnimationCoroutine());
    }

    public IEnumerator StartCurrentHeartAnimationCoroutine()
    {
        yield return new WaitForEndOfFrame();

        currentShownGameHearts[currentShownGameHearts.Count - 1].GetComponent<Animator>().SetTrigger(Config.ANIMATOR_START_CURRENT_HEART);
    }

    public IEnumerator LoseOneHeartCoroutine()
    {
        currentShownGameHearts[currentShownGameHearts.Count - 1].GetComponent<Animator>().SetTrigger(Config.ANIMATOR_DEATH_CURRENT_HEART);

        GameObject heartToDestroy = currentShownGameHearts[currentShownGameHearts.Count - 1].gameObject;
        currentShownGameHearts.RemoveAt(currentShownGameHearts.Count - 1);

        yield return new WaitForSeconds(Config.BIG_DELAY);

        Destroy(heartToDestroy);

        if (currentShownGameHearts.Count > 0)
            StartCurrentHeartAnimation();
    }

    private IEnumerator LoseRemainingHeartsCoroutine()
    {
        for (int i = currentShownGameHearts.Count - 1; i >= 0; i--)
        {
            GameObject heart = currentShownGameHearts[i];

            if (heart.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(Config.ANIMATOR_AWAIT_STATE_NAME))
                heart.GetComponent<Animator>().SetTrigger(Config.ANIMATOR_START_CURRENT_HEART);

            yield return null;

            StartCoroutine(LoseOneHeartCoroutine());

            yield return new WaitForSeconds(Config.BIG_DELAY);
        }
    }

    public void Damaged()
    {
        StartCoroutine(LoseOneHeartCoroutine());
    }

    public void PlayerAliveStatusChange()
    {
        if (playerDamageReceiver.IsAlive)
            RestartCurrentShownHearts();
        else
        {
            StartCoroutine(LoseRemainingHeartsCoroutine());
        }
    }

    
}
