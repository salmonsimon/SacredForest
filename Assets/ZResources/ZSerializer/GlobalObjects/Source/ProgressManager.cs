using System;
using ZSerializer;

[Serializable, SerializeGlobalData(GlobalDataType.PerSaveFile)]
public partial class ProgressManager
{
    public bool firstTimePlaying = true;

    public bool finishedRoute1 = false;
    public bool finishedRoute2 = false;
    public bool finishedRoute3 = false;
    public bool finishedRoute4 = false;

    public int enemiesKilledCount = 0;
    public int deathsCount = 0;
    public float timePlayed = 0;

    public void Reset()
    {
        firstTimePlaying = true;

        finishedRoute1 = false;
        finishedRoute2 = false;
        finishedRoute3 = false;
        finishedRoute4 = false;

        enemiesKilledCount = 0;
        deathsCount = 0;
        timePlayed = 0;
}
}
