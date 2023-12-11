using System;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public static Floor Instance { get; private set; }
    
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
    }


    private void ChangeFloorColor(GameObject floor)
    {
        float hue = UnityEngine.Random.Range(0f, 360f);
        float saturation = UnityEngine.Random.Range(0f, 150f);
        float value = UnityEngine.Random.Range(50f, 100f);

        Color color = Color.HSVToRGB(hue / 360f, saturation / 100f, value / 100f);
        var spawnedFloorMaterial = floor.GetComponent<MeshRenderer>().material;

        spawnedFloorMaterial.color = color;
        spawnedFloorMaterial.SetColor("_EmissionColor", color);
        if (GameManager.Instance.GetGameMode() == GameManager.GameMode.Specific)
        {
            DisplayFloorPath.Instance.recordFloorColor.Add(color);
        }
    }
}