using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PressPath : MonoBehaviour
{
    public static PressPath Instance { get; private set; }

    private int randomPressPathNumber;
    PressData[] pressData;

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
        // Addressables.CleanBundleCache();
        CheckPathFiles();
    }

    void CheckPathFiles()
    {
        string configFilePath = Path.Combine(Application.persistentDataPath, "PathFiles");
        if (!Directory.Exists(configFilePath))
        {
            Directory.CreateDirectory(configFilePath);
            StartCoroutine(GetPathFiles(configFilePath));
        }
        else
        {
            var files = Directory.EnumerateFiles(configFilePath, "*.json").ToList();
            randomPressPathNumber = UnityEngine.Random.Range(0, files.Count);
            string text = File.ReadAllText(files[randomPressPathNumber]);
            PressDataArray dataArray = JsonUtility.FromJson<PressDataArray>(text);
            pressData = dataArray.pressData;
        }
    }

    IEnumerator GetPathFiles(string configFilePath)
    {
        var asyncOperationHandle = Addressables.LoadAssetsAsync<TextAsset>("path", null);
        yield return asyncOperationHandle;
        if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            var textAssets = asyncOperationHandle.Result;
            foreach (var textAsset in textAssets)
            {
                string filePath = Path.Combine(configFilePath, textAsset.name + ".json");
                File.WriteAllText(filePath, textAsset.text);
            }
        }
    }


    public PressData[] GetDirection()
    {
        return pressData;
    }
}