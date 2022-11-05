[System.Serializable]
public struct Counters
{
    private int enemiesKilledCount;
    public int EnemiesKilledCount
    {
        get { return enemiesKilledCount; }
        set { enemiesKilledCount = value; }
    }

    private int deathsCount;
    public int DeathsCount
    {
        get { return deathsCount; }
        set { deathsCount = value; }
    }

    private float timePlayed;
    public float TimePlayed
    {
        get { return timePlayed; }
        set { timePlayed = value; }
    }
}
