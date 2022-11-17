using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    #region Choose Game Panel

    [Header("Choose Game Panel")]

    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject chooseGamePanel;
    [SerializeField] private GameObject newGameErrorPanel;

    [Space(5)]

    [SerializeField] private List<GameObject> savedGamesButtons;
    [SerializeField] private List<Text> killsTexts;
    [SerializeField] private List<Text> deathsTexts;
    [SerializeField] private List<Text> timePlayedTexts;

    [Space(5)]

    [SerializeField] private List<Button> chooseGameButtons;
    private List<Button> activeChooseGameButtons = new List<Button>();
    private bool onChooseGamePanel;
    private int buttonSelectionIndex = 0;
    private int maxButtonSelectionIndex = 0;

    #endregion

    #region Saved Games 

    [Space(10)]
    [Header("Saved Game Panels")]
    [Space(2)]

    [SerializeField] private List<GameObject> savedGamePanels;
    [SerializeField] private List<Text> savedGameKillsTexts;
    [SerializeField] private List<Text> savedGameDeathsTexts;
    [SerializeField] private List<Text> savedGameTimePlayedTexts;

    #endregion

    #region Settings Panel

    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    #endregion

    private void Start()
    {
        ResetMainMenu();
    }

    private void Update()
    {
        if (onChooseGamePanel)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                if (buttonSelectionIndex < maxButtonSelectionIndex - 1)
                    buttonSelectionIndex++;
                else
                    buttonSelectionIndex = 0;

                activeChooseGameButtons[buttonSelectionIndex].Select();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                if (buttonSelectionIndex > 0)
                    buttonSelectionIndex--;
                else
                    buttonSelectionIndex = maxButtonSelectionIndex - 1;

                activeChooseGameButtons[buttonSelectionIndex].Select();
            }
        }
    }

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
            newGameErrorPanel.transform.Find("Exit").GetComponent<Button>().Select();
        }
        else
        {
            chooseGamePanel.SetActive(false);
            SetOnChooseGamePanel(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DisplaySavedGames()
    {
        SetOnChooseGamePanel(true);

        int savedGamesAmount = Settings.Instance.savedGamesAmount;

        foreach (GameObject savedGameButton in savedGamesButtons)
            savedGameButton.SetActive(false);

        for (int i = 0; i < savedGamesAmount; i++)
        {
            savedGamesButtons[i].SetActive(true);

            int savedGameIndex = Settings.Instance.currentSavedGames[i];

            killsTexts[i].text = Settings.Instance.savedGamesKillsCounter[savedGameIndex].ToString("#,##0");
            deathsTexts[i].text = Settings.Instance.savedGamesDeathsCounter[savedGameIndex].ToString("#,##0");

            float timePlayedFloat = Settings.Instance.savedGamesTimePlayedCounter[savedGameIndex];
            string time = GameManager.instance.FloatToTimeFormat(timePlayedFloat);
            timePlayedTexts[i].text = time;
        }

        CountActiveChooseGamePanelButtons();
    }

    public void DisplaySavedGame(int savedGameIndex)
    {
        SetOnChooseGamePanel(false);

        savedGamePanels[savedGameIndex].SetActive(true);

        int correctedSavedGameIndex = Settings.Instance.currentSavedGames[savedGameIndex];

        savedGameKillsTexts[savedGameIndex].text = Settings.Instance.savedGamesKillsCounter[correctedSavedGameIndex].ToString("#,##0");
        savedGameDeathsTexts[savedGameIndex].text = Settings.Instance.savedGamesDeathsCounter[correctedSavedGameIndex].ToString("#,##0");

        float timePlayedFloat = Settings.Instance.savedGamesTimePlayedCounter[correctedSavedGameIndex];
        string time = GameManager.instance.FloatToTimeFormat(timePlayedFloat);
        savedGameTimePlayedTexts[savedGameIndex].text = time;
    }

    public void ResetMainMenu()
    {
        SetOnChooseGamePanel(false);
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

        mainPanel.SetActive(true);
        playButton.Select();
    }

    private void CountActiveChooseGamePanelButtons()
    {
        maxButtonSelectionIndex = 0;
        buttonSelectionIndex = 0;
        activeChooseGameButtons.Clear();

        foreach (Button button in chooseGameButtons)
        {
            if (button.gameObject.activeSelf)
            {
                maxButtonSelectionIndex++;
                activeChooseGameButtons.Add(button);
            }
        }
    }

    private void SetOnChooseGamePanel(bool value)
    {
        onChooseGamePanel = value;
    }

    public void SetAudioSlidersVolumes()
    {
        musicVolumeSlider.value = Settings.Instance.musicVolume;
        sfxVolumeSlider.value = Settings.Instance.SFXVolume;
    }
}
