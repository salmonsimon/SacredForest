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
    [SerializeField] private BloodManager bloodManager;
    [SerializeField] private AnimationManager animationManager;

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
            Destroy(bloodManager.gameObject);
            Destroy(animationManager.gameObject);
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
        GameObject playerSpawnPoint = GameObject.FindGameObjectWithTag(Config.SPAWN_POINT_TAG);
        if (playerSpawnPoint)
            player.transform.position = playerSpawnPoint.transform.position;

        GameObject virtualCameraGameObject = GameObject.FindGameObjectWithTag(Config.CINEMACHINE_CAMERA_TAG);

        if (virtualCameraGameObject)
        {
            Cinemachine.CinemachineVirtualCamera virtualCamera = virtualCameraGameObject.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            virtualCamera.Follow = player.transform;
        }

        levelLoader.FinishTransition();
    }

    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration, Transform parent = null)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration, parent);
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

    public void SetIsTeleporting(bool value)
    {
        isTeleporting = value;
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

    public BloodManager GetBloodManager()
    {
        return bloodManager;
    }

    public AnimationManager GetAnimationManager()
    {
        return animationManager;
    }

    #endregion
}
