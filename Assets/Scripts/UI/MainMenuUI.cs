using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    #region Choose Game Panel

    [SerializeField] private GameObject chooseGamePanel;
    [SerializeField] private GameObject newGameErrorPanel;

    [SerializeField] private List<GameObject> savedGamesButtons;
    [SerializeField] private List<Text> killsTexts;
    [SerializeField] private List<Text> deathsTexts;
    [SerializeField] private List<Text> timePlayedTexts;

    #endregion

    #region Saved Games 

    [SerializeField] private List<GameObject> savedGamePanels;
    [SerializeField] private List<Text> savedGameKillsTexts;
    [SerializeField] private List<Text> savedGameDeathsTexts;
    [SerializeField] private List<Text> savedGameTimePlayedTexts;

    #endregion

    public void PlayGame(int savedGameIndex)
    {
        GameManager.instance.SetIsOnMainMenu(false);

        ZSerializer.ZSerializerSettings.Instance.selectedSaveFile = savedGameIndex;
        ProgressManager.Load();

        GameManager.instance.GetCurrentProgressManager().Initialize();
        GameManager.instance.GetCountersUI().UpdateCounters();

        GameManager.instance.GetLevelLoader().LoadLevel(Config.MAIN_SCENE_NAME, Config.CROSSFADE_TRANSITION);
    }

    public void PlayGameCorrected(int savedGameIndex)
    {
        GameManager.instance.SetIsOnMainMenu(false);

        int correctedIndex = Settings.Instance.currentSavedGames[savedGameIndex];

        ZSerializer.ZSerializerSettings.Instance.selectedSaveFile = correctedIndex;
        ProgressManager.Load();

        GameManager.instance.GetCurrentProgressManager().Initialize();
        GameManager.instance.GetCountersUI().UpdateCounters();

        GameManager.instance.GetLevelLoader().LoadLevel(Config.MAIN_SCENE_NAME, Config.CROSSFADE_TRANSITION);
    }

    public void NewGame()
    {
        bool couldCreateNewGame = Settings.Instance.AddNewGameAndPlay();

        if (!couldCreateNewGame)
        {
            newGameErrorPanel.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DisplaySavedGames()
    {
        int savedGamesAmount = Settings.Instance.savedGamesAmount;

        foreach (GameObject savedGameButton in savedGamesButtons)
            savedGameButton.SetActive(false);

        for (int i = 0; i < savedGamesAmount; i++)
        {
            savedGamesButtons[i].SetActive(true);

            int savedGameIndex = Settings.Instance.currentSavedGames[i];

            killsTexts[i].text = Settings.Instance.savedGamesCounters[savedGameIndex].EnemiesKilledCount.ToString("#,##0");
            deathsTexts[i].text = Settings.Instance.savedGamesCounters[savedGameIndex].DeathsCount.ToString("#,##0");

            float timePlayedFloat = Settings.Instance.savedGamesCounters[savedGameIndex].TimePlayed;
            string time = GameManager.instance.FloatToTimeFormat(timePlayedFloat);
            timePlayedTexts[i].text = time;
        }
    }

    public void DisplaySavedGame(int savedGameIndex)
    {
        savedGamePanels[savedGameIndex].SetActive(true);

        int correctedSavedGameIndex = Settings.Instance.currentSavedGames[savedGameIndex];

        savedGameKillsTexts[savedGameIndex].text = Settings.Instance.savedGamesCounters[correctedSavedGameIndex].EnemiesKilledCount.ToString("#,##0");
        savedGameDeathsTexts[savedGameIndex].text = Settings.Instance.savedGamesCounters[correctedSavedGameIndex].DeathsCount.ToString("#,##0");

        float timePlayedFloat = Settings.Instance.savedGamesCounters[correctedSavedGameIndex].TimePlayed;
        string time = GameManager.instance.FloatToTimeFormat(timePlayedFloat);
        savedGameTimePlayedTexts[savedGameIndex].text = time;
    }

    public void ResetMainMenu()
    {
        chooseGamePanel.SetActive(false);

        for (int i = 0; i < savedGamePanels.Count; i++)
        {
            savedGamePanels[i].SetActive(false);
        }

        int savedGamesAmount = Settings.Instance.savedGamesAmount;

        for (int i = 0; i < savedGamesAmount; i++)
        {
            savedGamesButtons[i].SetActive(false);
        }
    }
}
