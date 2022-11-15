using System;
using System.Collections.Generic;
using ZSerializer;

[Serializable, SerializeGlobalData(GlobalDataType.Globally)]
public partial class Settings
{
    public int newGameIndex = 0;
    public int savedGamesAmount = 0;
    public List<int> currentSavedGames = new List<int>();

    public ZDictionary<int, int> savedGamesKillsCounterSerialized = new ZDictionary<int, int>();
    public Dictionary<int, int> savedGamesKillsCounter = new Dictionary<int, int>();

    public ZDictionary<int, int> savedGamesDeathsCounterSerialized = new ZDictionary<int, int>();
    public Dictionary<int, int> savedGamesDeathsCounter = new Dictionary<int, int>();

    public ZDictionary<int, float> savedGamesTimePlayedCounterSerialized = new ZDictionary<int, float>();
    public Dictionary<int, float> savedGamesTimePlayedCounter = new Dictionary<int, float>();

    public float musicVolume = 1;
    public float SFXVolume = 1;

    public void SaveCounters(int enemiesKilledCount, int deathsCount, float timePlayed)
    {
        savedGamesKillsCounter[ZSerializerSettings.Instance.selectedSaveFile] = enemiesKilledCount;
        savedGamesKillsCounterSerialized = savedGamesKillsCounter;

        savedGamesDeathsCounter[ZSerializerSettings.Instance.selectedSaveFile] = deathsCount;
        savedGamesDeathsCounterSerialized = savedGamesDeathsCounter;

        savedGamesTimePlayedCounter[ZSerializerSettings.Instance.selectedSaveFile] = timePlayed;
        savedGamesTimePlayedCounterSerialized = savedGamesTimePlayedCounter;
    }

    public void Deserialize()
    {
        for (var i = 0; i < currentSavedGames.Count; i++)
        {
            savedGamesKillsCounter.Add(savedGamesKillsCounterSerialized.keys[i], savedGamesKillsCounterSerialized.values[i]);
            savedGamesDeathsCounter.Add(savedGamesDeathsCounterSerialized.keys[i], savedGamesDeathsCounterSerialized.values[i]);
            savedGamesTimePlayedCounter.Add(savedGamesTimePlayedCounterSerialized.keys[i], savedGamesTimePlayedCounterSerialized.values[i]);
        }
    }

    public void ResetAudioSettingsToDefault()
    {
        musicVolume = 1;
        SFXVolume = 1;
    }

    public bool AddNewGameAndPlay()
    {
        if (savedGamesAmount >= 3)
        {
            return false;
        }
        else
        {
            currentSavedGames.Add(newGameIndex);

            AddNewCounters(newGameIndex);

            ZSerializerSettings.Instance.selectedSaveFile = newGameIndex;
            ProgressManager.Save();
            GameManager.instance.GetCurrentProgressManager().Initialize();

            savedGamesAmount++;
            newGameIndex++;

            Save();

            GameManager.instance.GetMainMenuUI().PlayGame(newGameIndex - 1);
        }

        return true;
    }

    public void DeleteGame(int savedGameIndex)
    {
        int correctedIndex = currentSavedGames[savedGameIndex];

        ZSerialize.DeleteSaveFile(correctedIndex);

        currentSavedGames.Remove(correctedIndex);

        RemoveCounters(correctedIndex);

        savedGamesAmount--;

        Save();
    }

    private void AddNewCounters(int newGameIndex)
    {
        savedGamesKillsCounter.Add(newGameIndex, 0);
        savedGamesKillsCounterSerialized = savedGamesKillsCounter;

        savedGamesDeathsCounter.Add(newGameIndex, 0);
        savedGamesDeathsCounterSerialized = savedGamesDeathsCounter;

        savedGamesTimePlayedCounter.Add(newGameIndex, 0);
        savedGamesTimePlayedCounterSerialized = savedGamesTimePlayedCounter;
    }

    private void RemoveCounters(int correctedGameIndex)
    {
        savedGamesKillsCounter.Remove(correctedGameIndex);
        savedGamesKillsCounterSerialized = savedGamesKillsCounter;

        savedGamesDeathsCounter.Remove(correctedGameIndex);
        savedGamesDeathsCounterSerialized = savedGamesDeathsCounter;

        savedGamesTimePlayedCounter.Remove(correctedGameIndex);
        savedGamesTimePlayedCounterSerialized = savedGamesTimePlayedCounter;
    }
}
