using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Floor : MonoBehaviour
{
    public static Floor Instance { get; private set; }

    private Material spawnedFloorMaterial;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnChangeFloorColor += GameManager_OnChangeFloorColor;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnChangeFloorColor -= GameManager_OnChangeFloorColor;
    }

    private void GameManager_OnChangeFloorColor(object sender, EventArgs e)
    {
        GameObject floor = sender as GameObject;
        ChangeFloorColor(floor);
        // Debug.Log("我的天" + recordFloorColor.Count);
    }


    private void ChangeFloorColor(GameObject floor)
    {
        spawnedFloorMaterial = floor.GetComponent<MeshRenderer>().material;
        float hue = UnityEngine.Random.Range(0f, 360f);
        float saturation = UnityEngine.Random.Range(0f, 150f);
        float value = UnityEngine.Random.Range(50f, 100f);

        Color color = Color.HSVToRGB(hue / 360f, saturation / 100f, value / 100f);
        spawnedFloorMaterial.color = color;
            DisplayFloorPath.Instance.recordFloorColor.Add(color);
    }

    // public List<Color> GetRecordColor()
    // {
    //     return recordFloorColor;
    // }
}