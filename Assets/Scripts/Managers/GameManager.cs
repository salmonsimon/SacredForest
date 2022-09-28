using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    #region GameObjects

    [SerializeField] private GameObject player;
    [SerializeField] private LevelLoader levelLoader;

    #endregion

    #region Logic Variables

    private bool isGamePaused;

    #endregion

    private void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(levelLoader.gameObject);
            Destroy(player.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player.transform.position = GameObject.FindGameObjectWithTag(Config.SPAWN_POINT_TAG).transform.position;

        StartCoroutine(levelLoader.FinishTransition());
    }


    #region Getters and Setters

    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    public void SetGamePaused(bool value)
    {
        isGamePaused = value;

        if (value)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    public LevelLoader GetLevelLoader()
    {
        return levelLoader;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    #endregion
}
