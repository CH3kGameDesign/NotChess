using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class PerformanceTracker
{
    private const string fileName = "AW_PerformanceOutput.csv";

    private const bool sendToGoogle = true;
    private const string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSe2S2IdlyhWV4A28Z4Hs3jfiBEIC0ModPlmBVqcko47S6Pipw/formResponse";

    static List<TrackedData> trackedData = new List<TrackedData>();
    
    public static void TrackData(TrackedData _data)
    {
        trackedData.Add(_data);
    }
    public static void ClearData()
    {
        trackedData.Clear();
    }
    
    private static void SaveToDownloads(string directoryPath, string fileName)
    {
        string filePath = Path.Combine(directoryPath, fileName);
        var csvGenerator = new CsvGenerator<TrackedData>();
        List<string> _csv = csvGenerator.GenerateCsv(trackedData, fileName);
        File.WriteAllLines(filePath, _csv);
    }

    public static void SendToGoogle()
    {
        if (sendToGoogle && trackedData.Count>1)
        {
            WWWForm form = new WWWForm();
            string temp = "";
            var csvGenerator = new CsvGenerator<TrackedData>();
            List<string> _csv = csvGenerator.GenerateCsv(trackedData, fileName);
            foreach (var item in _csv)
            {
                temp += item + "\n";
            }
            form.AddField("entry.1355281758", temp);
            byte[] rawData = form.data;
            GoogleFormExport.Export(BASE_URL, rawData);
        }
        ClearData();
    }
}

public struct TrackedData
{
    public string BuildInfo { get; private set; }
    public string GameType { get; private set; }
    public string DeviceInfo { get; private set; }
    public string FPS { get; private set; }
    public bool CheatsUsed { get; private set; }
    public string TimeScale { get; private set; }
    public string ActiveTroops { get; private set; }
    public int ParticleCount { get; private set; }

    public TrackedData(string _buildInfo, string _gameType, string _deviceInfo, string _fps, bool _cheatsUsed, string _timeScale, string _activeTroops, int _particleCount)
    {
        BuildInfo = _buildInfo;
        GameType = _gameType;
        DeviceInfo = _deviceInfo;
        FPS = _fps;
        CheatsUsed = _cheatsUsed;
        TimeScale = _timeScale;
        ActiveTroops = _activeTroops;
        ParticleCount = _particleCount;
    }
}