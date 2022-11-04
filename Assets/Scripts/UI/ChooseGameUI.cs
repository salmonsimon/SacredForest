using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseGameUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> savedGamesButtons;
    [SerializeField] private List<Text> killsTexts;
    [SerializeField] private List<Text> deathsTexts;
    [SerializeField] private List<Text> timePlayedTexts;

    public void DisplaySavedGames()
    {
        int savedGamesAmount = Settings.Instance.savedGamesAmount;

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
}
