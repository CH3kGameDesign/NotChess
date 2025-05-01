using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (GoogleFormExport))]
public class AnalyticsHandler : MonoBehaviour
{

    public bool sendAnalytics = true;

    public static AnalyticsHandler Instance;
    GoogleFormExport GFE;
    float updateTimer = 0f;
    float updateDelay = 1f;

    float averageFPS = 0f;
    List<float> FPS = new List<float>();
    int FPSListLimit = 20;

    void Start()
    {
        Instance = this;
        GFE = GetComponent<GoogleFormExport>();
    }

    private void Update()
    {
        FPSUpdate();

        if (updateTimer >= updateDelay)
        {
            updateTimer = 0;
            TrackData();
        }
        updateTimer += Time.unscaledDeltaTime;
    }

    void FPSUpdate()
    {
        FPS.Add(1 / Time.deltaTime);
        if (FPS.Count > FPSListLimit)
            FPS.RemoveAt(0);
        averageFPS = 0;
        foreach (var item in FPS)
        {
            averageFPS += item;
        }
        averageFPS /= FPS.Count;
    }

    public void TrackData()
    {
        int particleCount = 0;
        foreach (var item in FindObjectsOfType<ParticleSystem>())
            particleCount += item.particleCount;
        string gameType = $"Early Prototype";
        /*
        if (StaticData.testEnvironment)
            gameType = $"Testing Environment";

        TrackedData _trackedData = new TrackedData
            (
                _buildInfo: $"buildVersion:{Application.version}|timeDate:{System.DateTime.Now.ToString("yyyy/MM/dd h:mm:ss tt")}",
                _gameType: gameType,
                _deviceInfo: $"onDevice:{!Application.isEditor}|device:{SystemInfo.deviceModel}",
                _fps: averageFPS.ToString("F0"),
                _cheatsUsed: GameManager.Instance.DEBUG_cheatsUsed,
                _timeScale: Time.timeScale.ToString("F2"),
                _activeTroops: (GameManager.Instance.playerTroops.Count + GameManager.Instance.enemyTroops.Count).ToString(),
                _particleCount: particleCount
            );
        PerformanceTracker.TrackData(_trackedData);
        */
    }

    public void SendAnalytics()
    {
        if (sendAnalytics)
            PerformanceTracker.SendToGoogle();
    }

    public void OnApplicationQuit()
    {
        SendAnalytics();
    }
}
