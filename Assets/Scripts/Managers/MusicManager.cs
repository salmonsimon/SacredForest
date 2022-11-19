using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;
    private float musicVolume = 1f;

    private float playDelay = Config.BIG_DELAY;

    [Header("Single Tracks")]
    [Space(2)]

    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip mainAreaMusic;
    [SerializeField] private AudioClip firstBossMusic;
    [SerializeField] private AudioClip foxesMusic;

    [Space(10)]
    [Header("Layered Tracks")]
    [Space(2)]

    [SerializeField] private AudioClip[] firstFightingRouteTracks;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        UpdateVolume(Settings.Instance.musicVolume);
    }

    public void UpdateVolume(float value)
    {
        musicVolume = value;

        audioSource.volume = musicVolume;
        Settings.Instance.musicVolume = musicVolume;

        Settings.Save();
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
        PlayMusic(scene.name);
    }

    public void PlayMusic(string sceneName)
    {
        switch (sceneName)
        {
            case Config.MAIN_MENU_SCENE_NAME:

                audioSource.loop = true;
                audioSource.clip = mainMenuMusic;
                StartCoroutine(WaitAndPlay(playDelay));

                break;

            case Config.MAIN_SCENE_NAME:

                audioSource.loop = true;
                audioSource.clip = mainAreaMusic;
                StartCoroutine(WaitAndPlay(playDelay));

                break;

            case Config.FIGHTING_MECANICS_SCENE_NAME:
                break;

            case Config.FIRST_ROUTE_SCENE_NAME:

                audioSource.loop = false;
                StartCoroutine(PlayFirstFightingRouteMusic());

                break;

            case Config.FOXES_HOME_SCENE_NAME:

                audioSource.loop = true;
                audioSource.clip = foxesMusic;
                StartCoroutine(WaitAndPlay(playDelay));

                break;

            case Config.FIRST_BOSS_FRAME:

                audioSource.loop = true;
                audioSource.clip = firstBossMusic;
                StartCoroutine(WaitAndPlay(playDelay));

                break;
        }
    }

    public void StopMusic()
    {
        StopAllCoroutines();
        audioSource.Stop();
    }

    private IEnumerator PlayFirstFightingRouteMusic()
    {
        audioSource.Stop();

        double startTime = AudioSettings.dspTime + playDelay;

        for (int i = 0; i < firstFightingRouteTracks.Length; i++)
        {
            while (audioSource.isPlaying)
                yield return null;

            AudioClip clipToSchedule = firstFightingRouteTracks[i];

            audioSource.clip = clipToSchedule;
            audioSource.PlayScheduled(startTime);

            double duration = (double)clipToSchedule.samples / clipToSchedule.frequency;

            startTime += duration;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }

        while (audioSource.isPlaying)
            yield return null;

        StartCoroutine(PlayFirstFightingRouteMusic());
    }

    private IEnumerator WaitAndPlay(float duration)
    {
        yield return new WaitForSeconds(duration);

        audioSource.Play();
    }

    public string GetCurrentAudioClipName()
    {
        if (audioSource.isPlaying)
            return audioSource.clip.name;

        return "None";
    }

    public void SetLooping(bool value)
    {
        audioSource.loop = value;
    }
}
