using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountersUI : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private Text killsText;
    [SerializeField] private Text deathsText;
    [SerializeField] private Text timePlayedText;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        UpdateCounters();
    }

    private void Update()
    {
        timePlayedText.text = GameManager.instance.GetCurrentProgressManager().ShowCurrentTimePlayed();
    }

    public void UpdateCounters()
    {
        int currentKilledCount = GameManager.instance.GetCurrentProgressManager().GetCurrentEnemiesKilledCount();
        int currentDeathsCount = GameManager.instance.GetCurrentProgressManager().GetCurrentDeathsCount();

        killsText.text = currentKilledCount.ToString("#,##0");
        deathsText.text = currentDeathsCount.ToString("#,##0");
        timePlayedText.text = GameManager.instance.GetCurrentProgressManager().ShowCurrentTimePlayed();
    }

    public void ShowCounters(bool value)
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
}
