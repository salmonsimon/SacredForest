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
    [SerializeField] private FloatingTextManager floatingTextManager;
    [SerializeField] private CinemachineShake cinemachineShake;

    #endregion

    #region Logic Variables

    private bool isGamePaused;
    private bool isTeleporting;

    #endregion

    private void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            Destroy(levelLoader.gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject);
            Destroy(cinemachineShake.gameObject);
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

        Cinemachine.CinemachineVirtualCamera virtualCamera = GameObject.FindGameObjectWithTag(Config.CINEMACHINE_CAMERA_TAG).GetComponent<Cinemachine.CinemachineVirtualCamera>();
        virtualCamera.Follow = player.transform;

        StartCoroutine(levelLoader.FinishTransition());
    }

    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }

    #region Getters and Setters

    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    public bool IsTeleporting()
    {
        return isTeleporting;
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

    public CinemachineShake GetCinemachineShake()
    {
        return cinemachineShake;
    }

    #endregion
}
