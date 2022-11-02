using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CinemachineShake : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    private NoiseSettings noiseSettings;

    private float startingIntensity;

    private float shakeTimer;
    private float shakeTimerTotal;

    private void Awake()
    {
        noiseSettings = Resources.Load(Config.SHAKE_FILE) as NoiseSettings;
    }

    private void Start()
    {
        SetVirtualCamera();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetVirtualCamera();
    }

    public void SetVirtualCamera()
    {
        CinemachineVirtualCamera[] virtualCameras = FindObjectsOfType<CinemachineVirtualCamera>();

        for (int i = 0; i < virtualCameras.Length; i++)
        {
            if (i == 0)
            {
                cinemachineVirtualCamera = virtualCameras[i];
            }
            else if (virtualCameras[i].Priority > cinemachineVirtualCamera.Priority)
            {
                cinemachineVirtualCamera = virtualCameras[i];
            }
        }

        if (cinemachineVirtualCamera)
        {
            cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            cinemachineBasicMultiChannelPerlin.m_NoiseProfile = noiseSettings;
        }
        
    }

    public void ShakeCamera(float intensity, float time)
    {
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(0f, startingIntensity, shakeTimer / shakeTimerTotal);
        }
    }
}
