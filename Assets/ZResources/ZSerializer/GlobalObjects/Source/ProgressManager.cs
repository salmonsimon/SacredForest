using System;
using ZSerializer;

[Serializable, SerializeGlobalData(GlobalDataType.PerSaveFile)]
public partial class ProgressManager
{
    public int difficultyLevel = -1;

    public bool firstTimePlaying = true;

    public bool finishedRoute1 = false;
    public bool route1BossHasTransformed = false;

    public bool finishedRoute2 = false;
    public bool finishedRoute3 = false;
    public bool finishedRoute4 = false;

    public int enemiesKilledCount = 0;
    public int deathsCount = 0;
    public float timePlayed = 0;

    public void Reset()
    {
        difficultyLevel = -1;

        firstTimePlaying = true;

        finishedRoute1 = false;
        route1BossHasTransformed = false;

        finishedRoute2 = false;
        finishedRoute3 = false;
        finishedRoute4 = false;

        enemiesKilledCount = 0;
        deathsCount = 0;
        timePlayed = 0;
}
}
