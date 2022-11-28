using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private CurrentProgressManager currentProgressManager;

    #region UI

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private MainMenuUI mainMenuUI;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseMenuFightingRoute;
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
            Destroy(musicManager.gameObject);
            Destroy(currentProgressManager.gameObject);

            Destroy(mainMenu.gameObject);
            Destroy(mainMenuUI.gameObject);
            Destroy(pauseMenu.gameObject);
            Destroy(pauseMenuFightingRoute.gameObject);
            Destroy(countersUI.gameObject);
        }
        else
        {
            instance = this;

            ProgressManager.Instance.Reset();

            Settings.Load();
            Settings.Instance.Deserialize();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
            if (scene.name == Config.MAIN_SCENE_NAME)
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
            pauseMenuFightingRoute.SetActive(false);
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
        animationManager.ClearCanvases();
        dialogueManager.ClearDialogues();
        player.GetComponent<Animator>().runtimeAnimatorController = Resources.Load(Config.PLAYER_ANIMATOR_CONTROLLER_FILE) as RuntimeAnimatorController;

        StartCoroutine(levelLoader.CinematicBracketsEnd());

        SetGamePaused(false);
        player.SetActive(false);

        currentProgressManager.SaveCurrentProgress();

        isOnMainMenu = true;
        currentProgressManager.UpdateCurrentFightingRoute(FightingRoute.None);

        ZSerializer.ZSerializerSettings.Instance.selectedSaveFile = -1;
        ProgressManager.Instance.Reset();

        levelLoader.LoadLevel(Config.MAIN_MENU_SCENE_NAME, Config.CROSSFADE_TRANSITION);
        pauseMenu.SetActive(false);
        pauseMenuFightingRoute.SetActive(false);
    }

    public void PauseGame()
    {
        GetSFXManager().PlaySound(Config.PAUSE_SFX);
        SetGamePaused(true);

        if (currentProgressManager.CurrentFightingRoute == FightingRoute.None)
        {
            pauseMenu.SetActive(true);
            pauseMenu.transform.Find("Pause Panel").gameObject.SetActive(true);
            pauseMenu.GetComponent<ButtonSelection>().ResetSelectedButton();
        }
        else
        {
            pauseMenuFightingRoute.SetActive(true);
            pauseMenuFightingRoute.transform.Find("Pause Panel").gameObject.SetActive(true);
            pauseMenuFightingRoute.GetComponent<ButtonSelection>().ResetSelectedButton();
        }
        
    }

    public void ResumeGame()
    {
        SetGamePaused(false);

        if (currentProgressManager.CurrentFightingRoute == FightingRoute.None)
        {
            pauseMenu.SetActive(false);
            pauseMenu.transform.Find("Settings Panel").gameObject.SetActive(false);
        }
        else
        {
            pauseMenuFightingRoute.SetActive(false);
            pauseMenuFightingRoute.transform.Find("Settings Panel").gameObject.SetActive(false);
        }
    }

    public void RestartFrame()
    {
        FrameManager frameManager = GameObject.FindGameObjectWithTag(Config.FRAME_MANAGER_TAG).GetComponent<FrameManager>();

        dialogueManager.ClearDialogues();
        animationManager.ClearCanvases();
        StartCoroutine(frameManager.RestartFrame());
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

    public MusicManager GetMusicManager()
    {
        return musicManager;
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

    public void SetAudioSlidersVolumesPauseMenu()
    {
        Slider musicVolumeSlider = null;
        Slider sfxVolumeSlider = null;

        if (currentProgressManager.CurrentFightingRoute == FightingRoute.None)
        {
            musicVolumeSlider = GameObject.Find("Pause Menu UI/Settings Panel/Music/Music Slider").GetComponent<Slider>();
            sfxVolumeSlider = GameObject.Find("Pause Menu UI/Settings Panel/SFX/SFX Slider").GetComponent<Slider>();
        }
        else
        {
            musicVolumeSlider = GameObject.Find("Pause Menu UI - Fighting Route/Settings Panel/Music/Music Slider").GetComponent<Slider>();
            sfxVolumeSlider = GameObject.Find("Pause Menu UI - Fighting Route/Settings Panel/SFX/SFX Slider").GetComponent<Slider>();
        }

        musicVolumeSlider.value = Settings.Instance.musicVolume;
        sfxVolumeSlider.value = Settings.Instance.SFXVolume;
    }
}
