using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PressPath : MonoBehaviour
{
    public static PressPath Instance { get; private set; }

    private List<string> pressPathList;
    private int randomPressPathNumber;
    [Serializable]
    public struct PressData
    {
        public GameManager.SpawnFloorPosition direction;
    }


    [Serializable]
    public class PressDataArray
    {
        public PressData[] pressData;
    }


    private void Awake()
    {
        Instance = this;
        pressPathList = new List<string>();
        string configFilePath = Path.Combine(Application.dataPath, "ConfigFiles");
        var files = Directory.EnumerateFiles(configFilePath, "*.json");
        foreach (string file in files)
        {
            pressPathList.Add(file);
        }
        randomPressPathNumber = UnityEngine.Random.Range(0, pressPathList.Count);
    }
    

    public PressData[] GetDirection()
    {
        Debug.Log(randomPressPathNumber);
        string directionData = File.ReadAllText(pressPathList[randomPressPathNumber]);
        PressDataArray dataArray = JsonUtility.FromJson<PressDataArray>(directionData);
        return dataArray.pressData;
    }
}