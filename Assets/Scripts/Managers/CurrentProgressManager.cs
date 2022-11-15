using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CurrentProgressManager : MonoBehaviour
{
    [SerializeField] private bool firstTimePlaying;
    public bool FirstTimePlaying { get { return firstTimePlaying; } set { firstTimePlaying = value; }}

    [SerializeField] private FightingRoute currentFightingRoute;

    [SerializeField] private bool finishedRoute1;
    [SerializeField] private bool route1BossHasTransformed;

    [SerializeField] private bool finishedRoute2;
    [SerializeField] private bool finishedRoute3;
    [SerializeField] private bool finishedRoute4;

    #region Routes Properties

    public bool FinishedRoute1
    {
        get { return finishedRoute1; }
        private set
        {
            if (finishedRoute1 == value) return;

            finishedRoute1 = value;
            if (OnFirstRouteFinishedStateChange != null)
                OnFirstRouteFinishedStateChange();
        }
    }


    public bool Route1BossHasTransformed { get { return route1BossHasTransformed; } set { route1BossHasTransformed = value; } }


    public bool FinishedRoute2
    {
        get { return finishedRoute2; }
        private set
        {
            if (finishedRoute2 == value) return;

            finishedRoute2 = value;
            if (OnSecondRouteFinishedStateChange != null)
                OnSecondRouteFinishedStateChange();
        }
    }

    public bool FinishedRoute3
    {
        get { return finishedRoute3; }
        private set
        {
            if (finishedRoute3 == value) return;

            finishedRoute3 = value;
            if (OnThirdRouteFinishedStateChange != null)
                OnThirdRouteFinishedStateChange();
        }
    }

    public bool FinishedRoute4
    {
        get { return finishedRoute4; }
        private set
        {
            if (finishedRoute4 == value) return;

            finishedRoute4 = value;
            if (OnFourthRouteFinishedStateChange != null)
                OnFourthRouteFinishedStateChange();
        }
    }

    #endregion

    [SerializeField] private float timePlayed;

    [SerializeField] private int enemiesKilledCount;
    [SerializeField] private int deathsCount;

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

    public void Initialize()
    {
        ProgressManager.Load();

        firstTimePlaying = ProgressManager.Instance.firstTimePlaying;

        FinishedRoute1 = ProgressManager.Instance.finishedRoute1;

        if (FinishedRoute1)
        {
            Route1BossHasTransformed = true;
        }
        else
        {
            Route1BossHasTransformed = false;
        }

        FinishedRoute2 = ProgressManager.Instance.finishedRoute2;
        FinishedRoute3 = ProgressManager.Instance.finishedRoute3;
        FinishedRoute4 = ProgressManager.Instance.finishedRoute4;

        timePlayed = ProgressManager.Instance.timePlayed;

        enemiesKilledCount = ProgressManager.Instance.enemiesKilledCount;
        deathsCount = ProgressManager.Instance.deathsCount;
    }

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

    public void FinishedCurrentFightingRoute(bool value)
    {
        if (currentFightingRoute != FightingRoute.None)
        {
            switch (currentFightingRoute)
            {
                case FightingRoute.Route1:
                    FinishedRoute1 = value;
                    break;

                case FightingRoute.Route2:
                    FinishedRoute2 = value;
                    break;

                case FightingRoute.Route3:
                    FinishedRoute3 = value;
                    break;

                case FightingRoute.Route4:
                    FinishedRoute4 = value;
                    break;
            }
        }
    }

    public void UpdateCurrentFightingRoute(FightingRoute fightingRoute)
    {
        currentFightingRoute = fightingRoute;
    }

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

        ProgressManager.Instance.finishedRoute1 = FinishedRoute1;
        ProgressManager.Instance.route1BossHasTransformed = Route1BossHasTransformed;

        ProgressManager.Instance.finishedRoute2 = FinishedRoute2;
        ProgressManager.Instance.finishedRoute3 = FinishedRoute3;
        ProgressManager.Instance.finishedRoute4 = FinishedRoute4;

        ProgressManager.Instance.timePlayed = timePlayed;

        ProgressManager.Instance.enemiesKilledCount = enemiesKilledCount;
        ProgressManager.Instance.deathsCount = deathsCount;

        ProgressManager.Save();

        Settings.Instance.SaveCounters(enemiesKilledCount, deathsCount, timePlayed);

        Settings.Save();
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
        return GameManager.instance.FloatToTimeFormat(timePlayed);
    }

    #endregion
}
