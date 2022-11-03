using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentProgressManager : MonoBehaviour
{
    [SerializeField] private bool firstTimePlaying;

    [SerializeField] private FightingRoute currentFightingRoute;

    [SerializeField] private bool finishedRoute1;
    [SerializeField] private bool finishedRoute2;
    [SerializeField] private bool finishedRoute3;
    [SerializeField] private bool finishedRoute4;

    

    [SerializeField] private float timePlayed;

    [SerializeField] private int enemiesKilledCount;
    [SerializeField] private int deathsCount;

    private void Awake()
    {
        firstTimePlaying = ProgressManager.Instance.firstTimePlaying;

        finishedRoute1 = ProgressManager.Instance.finishedRoute1;
        finishedRoute2 = ProgressManager.Instance.finishedRoute2;
        finishedRoute3 = ProgressManager.Instance.finishedRoute3;
        finishedRoute4 = ProgressManager.Instance.finishedRoute4;

        timePlayed = ProgressManager.Instance.timePlayed;

        enemiesKilledCount = ProgressManager.Instance.enemiesKilledCount;
        deathsCount = ProgressManager.Instance.deathsCount;
    }

    #region Events and Delegates

    public delegate void OnFirstRouteFinishedStateChangeDelegate();
    public event OnFirstRouteFinishedStateChangeDelegate OnFirstRouteFinishedStateChange;

    public delegate void OnSecondRouteFinishedStateChangeDelegate();
    public event OnSecondRouteFinishedStateChangeDelegate OnSecondRouteFinishedStateChange;

    public delegate void OnThirdRouteFinishedStateChangeDelegate();
    public event OnFirstRouteFinishedStateChangeDelegate OnThirdRouteFinishedStateChange;

    public delegate void OnFourthRouteFinishedStateChangeDelegate();
    public event OnFirstRouteFinishedStateChangeDelegate OnFourthRouteFinishedStateChange;

    #endregion

    private void Update()
    {
        if (!GameManager.instance.IsOnMainMenu())
            timePlayed += Time.deltaTime;
    }

    #region Counters

    public void IncreaseEnemiesKilledCount()
    {
        enemiesKilledCount++;
        GameManager.instance.GetCountersUI().UpdateCounters();
    }

    public void IncreaseDeathsCount()
    {
        deathsCount++;
        GameManager.instance.GetCountersUI().UpdateCounters();
    }

    #endregion

    #region Fighting Routes

    public void FinishedRoute1(bool value)
    {
        finishedRoute1 = value;

        if (OnFirstRouteFinishedStateChange != null)
            OnFirstRouteFinishedStateChange();
    }

    public void FinishedRoute2(bool value)
    {
        finishedRoute2 = value;

        if (OnSecondRouteFinishedStateChange != null)
            OnSecondRouteFinishedStateChange();
    }

    public void FinishedRoute3(bool value)
    {
        finishedRoute3 = value;

        if (OnThirdRouteFinishedStateChange != null)
            OnThirdRouteFinishedStateChange();
    }

    public void FinishedRoute4(bool value)
    {
        finishedRoute4 = value;

        if (OnFourthRouteFinishedStateChange != null)
            OnFourthRouteFinishedStateChange();
    }

    public void FinishedCurrentFightingRoute(bool value)
    {
        if (currentFightingRoute != FightingRoute.None)
        {
            switch (currentFightingRoute)
            {
                case FightingRoute.Route1:
                    FinishedRoute1(value);
                    break;

                case FightingRoute.Route2:
                    FinishedRoute2(value);
                    break;

                case FightingRoute.Route3:
                    FinishedRoute3(value);
                    break;

                case FightingRoute.Route4:
                    FinishedRoute4(value);
                    break;
            }
        }
    }

    public void UpdateCurrentFightingRoute(FightingRoute fightingRoute)
    {
        currentFightingRoute = fightingRoute;
    }

    #region Public Getters

    public bool IsFinishedRoute1
    {
        get { return finishedRoute1; }
    }

    public bool IsFinishedRoute2
    {
        get { return finishedRoute2; }
    }

    public bool IsFinishedRoute3
    {
        get { return finishedRoute3; }
    }

    public bool IsFinishedRoute4
    {
        get { return finishedRoute4; }
    }

    #endregion

    #endregion

    public void ResetCounters()
    {
        enemiesKilledCount = ProgressManager.Instance.enemiesKilledCount;
        deathsCount = ProgressManager.Instance.deathsCount;

        FinishedCurrentFightingRoute(false);

        GameManager.instance.GetCountersUI().UpdateCounters();
    }

    public void SaveCurrentProgress()
    {
        ProgressManager.Instance.firstTimePlaying = firstTimePlaying;

        ProgressManager.Instance.finishedRoute1 = finishedRoute1;
        ProgressManager.Instance.finishedRoute2 = finishedRoute2;
        ProgressManager.Instance.finishedRoute3 = finishedRoute3;
        ProgressManager.Instance.finishedRoute4 = finishedRoute4;

        ProgressManager.Instance.timePlayed = timePlayed;

        ProgressManager.Instance.enemiesKilledCount = enemiesKilledCount;
        ProgressManager.Instance.deathsCount = deathsCount;

        ProgressManager.Save();
    }

    #region Show Counters

    public int GetCurrentEnemiesKilledCount()
    {
        return enemiesKilledCount;
    }

    public int GetCurrentDeathsCount()
    {
        return deathsCount;
    }

    public string ShowCurrentTimePlayed()
    {
        double timePlayedDouble = (double)timePlayed;

        System.TimeSpan time = System.TimeSpan.FromSeconds(timePlayedDouble);

        string displayTime = time.ToString("hh':'mm':'ss");

        return displayTime;
    }

    #endregion
}
