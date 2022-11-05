using System;
using System.Collections.Generic;
using ZSerializer;

[Serializable, SerializeGlobalData(GlobalDataType.Globally)]
public partial class Settings
{
    public int newGameIndex = 0;
    public int savedGamesAmount = 0;
    public List<int> currentSavedGames = new List<int>();

    public ZDictionary<int, Counters> savedGamesCountersSerialized = new ZDictionary<int, Counters>();
    public Dictionary<int, Counters> savedGamesCounters = new Dictionary<int, Counters>();

    public float musicVolume = 1;
    public float SFXVolume = 1;

    public void Deserialize()
    {
        for (var i = 0; i < savedGamesCountersSerialized.keys.Count; i++)
        {
            savedGamesCounters.Add(savedGamesCountersSerialized.keys[i], savedGamesCountersSerialized.values[i]);
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

            savedGamesCounters.Add(newGameIndex, new Counters());
            savedGamesCountersSerialized = savedGamesCounters;

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

        savedGamesCounters.Remove(correctedIndex);
        savedGamesCountersSerialized = savedGamesCounters;

        savedGamesAmount--;

        Save();
    }
}
