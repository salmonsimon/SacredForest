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
    [SerializeField] private CurrentProgressManager currentProgressManager;

    #region UI

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private MainMenuUI mainMenuUI;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private CountersUI countersUI;

    #endregion

    #endregion

    #region Logic Variables
    
    [SerializeField] private bool isOnMainMenu = true;

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
            Destroy(currentProgressManager.gameObject);

            Destroy(mainMenu.gameObject);
            Destroy(mainMenuUI.gameObject);
            Destroy(pauseMenu.gameObject);
            Destroy(countersUI.gameObject);
        }
        else
        {
            instance = this;

            ProgressManager.Instance.Reset();

            Settings.Load();
            Settings.Instance.Deserialize();
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
        if (!isGamePaused && !isOnMainMenu && Input.GetKeyDown(KeyCode.Escape))
            PauseGame();
        else if (isGamePaused && !isOnMainMenu && Input.GetKeyDown(KeyCode.Escape))
            ResumeGame();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        animationManager.ClearCanvases();

        if (!isOnMainMenu)
        {
            ZSerializer.ZSerialize.LoadScene();

            mainMenu.SetActive(false);

            countersUI.gameObject.SetActive(true);
            currentProgressManager.gameObject.SetActive(true);
            player.SetActive(true);

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
            player.SetActive(false);

            mainMenu.SetActive(true);

            pauseMenu.SetActive(false);
            countersUI.gameObject.SetActive(false);
            currentProgressManager.gameObject.SetActive(false);
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
        player.SetActive(false);

        currentProgressManager.SaveCurrentProgress();

        isOnMainMenu = true;
        currentProgressManager.UpdateCurrentFightingRoute(FightingRoute.None);

        ZSerializer.ZSerializerSettings.Instance.selectedSaveFile = -1;
        ProgressManager.Instance.Reset();

        levelLoader.LoadLevel(Config.MAIN_MENU_SCENE_NAME, Config.CROSSFADE_TRANSITION);
        pauseMenu.SetActive(false);
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

    public bool IsOnMainMenu()
    {
        return isOnMainMenu;
    }

    public void SetIsOnMainMenu(bool value)
    {
        isOnMainMenu = value;
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
        player.GetComponent<DamageReceiver>().Resurrect();

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

    public CurrentProgressManager GetCurrentProgressManager()
    {
        return currentProgressManager;
    }

    public CountersUI GetCountersUI()
    {
        return countersUI;
    }

    public MainMenuUI GetMainMenuUI()
    {
        return mainMenuUI;
    }

    #endregion

    public string FloatToTimeFormat(float timeInput)
    {
        double timePlayedDouble = (double)timeInput;

        System.TimeSpan time = System.TimeSpan.FromSeconds(timePlayedDouble);

        string displayTime = time.ToString("hh':'mm':'ss");

        return displayTime;
    }
}
