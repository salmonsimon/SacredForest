using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Cutscene05 : Cutscene
{
    private Transform slyDialogueTransform;
    private Transform akateDialogueTransform;

    [SerializeField] private GameObject sly;
    [SerializeField] private GameObject breakableBarrel;

    [SerializeField] private Cinemachine.CinemachineVirtualCamera playerCamera;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera slyCamera;

    [SerializeField] private FloatingUI jumpAndDashButtons;
    private bool finishedCutscene = false;
    private Coroutine showButtonCoroutine = null;

    private void Awake()
    {
        if (GameManager.instance.GetCurrentProgressManager().FinishedRoute1)
        {
            sly.SetActive(false);
            breakableBarrel.SetActive(false);

            this.enabled = false;
            this.gameObject.SetActive(false);
        }
    }

    protected override void Start()
    {
        base.Start();

        slyDialogueTransform = sly.transform.Find("DialogueBubbleTransform").transform;
        akateDialogueTransform = GameManager.instance.GetPlayer().transform.Find("DialogueBubbleTransform").transform;
        speakers = new List<Transform>() { slyDialogueTransform, akateDialogueTransform };
    }

    private void OnEnable()
    {
        GameObject.FindGameObjectWithTag("Breakable").GetComponent<DamageReceiver>().OnCharacterAliveStatusChange += BarrelBroken;
        showButtonCoroutine = StartCoroutine(WaitToShowButton());
    }
    private void OnDisable()
    {
        GameObject.FindGameObjectWithTag("Breakable").GetComponent<DamageReceiver>().OnCharacterAliveStatusChange -= BarrelBroken;
    }

    private void Update()
    {
        if (finishedCutscene && Input.GetKeyDown(KeyCode.C))
            GameManager.instance.GetAnimationManager().ClearWorldSpaceCanvas();
    }

    private IEnumerator Play()
    {
        GameManager.instance.GetMusicManager().PlayMusic(Config.FOXES_HOME_SCENE_NAME);

        GameManager.instance.GetLevelLoader().CinematicBracketsStart();

        GameObject player = GameManager.instance.GetPlayer();
        DeactivatePlayer(player);

        #region Rescued Scene - Suspecting

        yield return new WaitForSeconds(1f);

        playableDirector.playableAsset = timelines[0];

        Bind(playableDirector, "Sly Animations", sly);
        Bind(playableDirector, "Sly Movement", sly);

        playableDirector.extrapolationMode = DirectorWrapMode.None;
        playableDirector.Play();

        yield return new WaitForSeconds(1f);

        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[0], speakers, 0, 1);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        if (!GameManager.instance.GetDialogueManager().IsRunning && playableDirector.state == PlayState.Playing)
            playableDirector.Stop();

        #endregion

        #region Rescued Scene - Surprised

        yield return new WaitForSeconds(Config.SMALL_DELAY);

        playableDirector.playableAsset = timelines[1];

        Bind(playableDirector, "Sly Animations", sly);
        Bind(playableDirector, "Sly Movement", sly);

        playableDirector.extrapolationMode = DirectorWrapMode.Loop;
        playableDirector.Play();

        yield return new WaitForSeconds(Config.MEDIUM_DELAY);

        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[0], speakers, 2, 2);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        if (!GameManager.instance.GetDialogueManager().IsRunning && playableDirector.state == PlayState.Playing)
            playableDirector.Stop();

        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[0], speakers, 3, 4);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        #endregion

        #region Rescued Scene - Showing the Dash

        slyCamera.Priority = 1;
        playerCamera.Priority = 0;

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        playableDirector.extrapolationMode = DirectorWrapMode.None;
        playableDirector.playableAsset = timelines[2];

        Bind(playableDirector, "Sly Animations", sly);
        Bind(playableDirector, "Sly Movement", sly);

        playableDirector.Play();

        while (playableDirector.state == PlayState.Playing)
        {
            yield return null;
        }

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[0], speakers, 5, 5);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        playableDirector.playableAsset = timelines[3];

        Bind(playableDirector, "Sly Animations", sly);
        Bind(playableDirector, "Sly Movement", sly);

        playableDirector.Play();

        while (playableDirector.state == PlayState.Playing)
        {
            yield return null;
        }

        #endregion

        #region Rescued Scene - Farewell

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        GameManager.instance.GetDialogueManager().RunBubbleDialogue(dialogues[0], speakers, 6);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        playableDirector.playableAsset = timelines[4];

        Bind(playableDirector, "Sly Animations", sly);
        Bind(playableDirector, "Sly Movement", sly);

        playableDirector.Play();

        #endregion

        yield return new WaitForSeconds(1f);

        sly.SetActive(false);

        GameManager.instance.GetCurrentProgressManager().FinishedCurrentFightingRoute(true);

        slyCamera.Priority = 0;
        playerCamera.Priority = 1;

        StartCoroutine(GameManager.instance.GetLevelLoader().CinematicBracketsEnd());
        GameManager.instance.GetMusicManager().SetLooping(false);
        ActivatePlayer(player);

        yield return new WaitForSeconds(Config.SMALL_DELAY);

        GameManager.instance.GetAnimationManager().ShowImageUIWorldSpace(jumpAndDashButtons, new Vector3(player.transform.position.x, player.transform.position.y + .32f, player.transform.position.z), player.transform);

        finishedCutscene = true;
    }

    private void BarrelBroken()
    {
        if (showButtonCoroutine != null)
            StopCoroutine(showButtonCoroutine);

        GameManager.instance.GetAnimationManager().ClearWorldSpaceCanvas();

        StartCoroutine(Play());
    }

    private IEnumerator WaitToShowButton()
    {
        yield return null;

        yield return new WaitForSeconds(10f);

        GameObject barrel = GameObject.FindGameObjectWithTag("Breakable");
        GameManager.instance.GetAnimationManager().ShowImageUIWorldSpace(Config.X_KEY_GUI, new Vector3(barrel.transform.position.x, barrel.transform.position.y + .32f, barrel.transform.position.z));
    }
}
