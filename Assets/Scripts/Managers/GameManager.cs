using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private SFXManager sfxManager;

    #region UI

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;

    #endregion

    #endregion

    #region Logic Variables

    [SerializeField] private bool onMainMenu = true;

    [SerializeField] private bool isGamePaused;
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
            Destroy(dialogueManager.gameObject);
            Destroy(sfxManager.gameObject);

            Destroy(mainMenu.gameObject);
            Destroy(pauseMenu.gameObject);
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

    private void Update()
    {
        if (!isGamePaused && !onMainMenu && Input.GetKeyDown(KeyCode.Escape))
            PauseGame();
        else if (isGamePaused && !onMainMenu && Input.GetKeyDown(KeyCode.Escape))
            ResumeGame();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (!onMainMenu)
        {
            mainMenu.SetActive(false);

            GameObject playerSpawnPoint = GameObject.FindGameObjectWithTag(Config.SPAWN_POINT_TAG);
            if (playerSpawnPoint)
                player.transform.position = playerSpawnPoint.transform.position;

            GameObject virtualCameraGameObject = GameObject.FindGameObjectWithTag(Config.CINEMACHINE_CAMERA_TAG);

            if (virtualCameraGameObject)
            {
                Cinemachine.CinemachineVirtualCamera virtualCamera = virtualCameraGameObject.GetComponent<Cinemachine.CinemachineVirtualCamera>();
                virtualCamera.Follow = player.transform;
            }
        }
        else
        {
            mainMenu.SetActive(true);
            pauseMenu.SetActive(false);
        }

        levelLoader.FinishTransition();
    }

    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration, Transform parent = null)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration, parent);
    }

    public void ToMainMenu()
    {
        SetGamePaused(false);
        onMainMenu = true;

        levelLoader.LoadLevel(Config.MAIN_MENU_SCENE_NAME, Config.CROSSFADE_TRANSITION);
        pauseMenu.SetActive(false);
    }

    public void PlayGame()
    {
        onMainMenu = false;
        levelLoader.LoadLevel(Config.MAIN_SCENE_NAME, Config.CROSSFADE_TRANSITION);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DeleteSavedGame()
    {
        string path = Application.persistentDataPath;

        DirectoryInfo di = new DirectoryInfo(path);

        foreach (FileInfo file in di.EnumerateFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in di.EnumerateDirectories())
        {
            dir.Delete(true);
        }
    }

    public void PauseGame()
    {
        GetSFXManager().PlaySound(Config.PAUSE_SFX);
        SetGamePaused(true);

        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        SetGamePaused(false);

        pauseMenu.SetActive(false);
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

    public void RestartPlayer()
    {
        player.transform.rotation = Quaternion.identity;

        player.GetComponent<Animator>().keepAnimatorControllerStateOnDisable = false;

        player.gameObject.SetActive(false);

        player.GetComponent<PlayerMovementController>().Reset();
        player.GetComponent<PlayerAttackController>().Reset();

        player.gameObject.SetActive(true);
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

    public DialogueManager GetDialogueManager()
    {
        return dialogueManager;
    }

    public SFXManager GetSFXManager()
    {
        return sfxManager;
    }

    #endregion
}
