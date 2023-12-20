using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance {get; private set; }
    private CinemachineVirtualCamera cineCamera;
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startIntensity;

    void Awake()
    {
        Instance = this;
        cineCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCamera(float intensity, float time) {
        CinemachineBasicMultiChannelPerlin cinePerlin = cineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinePerlin.m_AmplitudeGain = intensity*1.5f;
        shakeTimer = time;
        shakeTimerTotal = time;


    }

    private void Update() {
        if (shakeTimer > 0) {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer < 0f) {
                CinemachineBasicMultiChannelPerlin cinePerlin = cineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cinePerlin.m_AmplitudeGain = Mathf.Lerp(startIntensity, 0f, 1-(shakeTimer/shakeTimerTotal));
            }
        }
    }
}
