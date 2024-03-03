using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DisplayFloorPath : MonoBehaviour
{
    public static DisplayFloorPath Instance { get; private set; }
    public List<Color> recordFloorColor = new();

    [SerializeField] private Image floorImage;
    [SerializeField] private Transform displayFloorPathUI;

    private float floorSize;
    private Vector3 floorPosition;
    private Color floorColor;
    private Image currentFloorImage;
    private RectTransform currentFloorImageRect;
    private bool displayCalled;
    private PressPath.PressData[] presData;
    private int recordFloorColorIndex;


    private void Awake()
    {
        Instance = this;
        // recordFloorColor = new List<Color>();
        floorSize = floorImage.rectTransform.rect.width;
        currentFloorImage = floorImage;
        currentFloorImageRect = floorImage.GetComponent<RectTransform>();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        presData = PressPath.Instance.GetDirection();

        Hide();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChange -= GameManager_OnGameStateChange;
    }

    private void GameManager_OnGameStateChange(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGameOver() && !displayCalled &&
            GameManager.Instance.GetGameMode() == GameManager.GameMode.Specific)
        {
            displayCalled = true;
            Display();
            CenterPath();
        }
    }

    private void GameManager_OnGameStarted(object sender, EventArgs e)
    {
        displayCalled = false;
    }

    private void Display()
    {
        Show();
        for (int i = 0; i < presData.Length; i++)
        {
            SpawnFloorWithPressPath(presData[i].direction);
        }
    }

    // 找出这个路线图中中心坐标，然后所有的图像左边都减去这个坐标，这样真个路线图就会居中
    private void CenterPath()
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        // Find the bounds of the path.
        foreach (RectTransform child in displayFloorPathUI)
        {
            Vector2 position = child.anchoredPosition;
            minX = Mathf.Min(minX, position.x);
            maxX = Mathf.Max(maxX, position.x);
            minY = Mathf.Min(minY, position.y);
            maxY = Mathf.Max(maxY, position.y);
        }

        // 计算中心坐标
        Vector2 center = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);

        foreach (RectTransform child in displayFloorPathUI)
        {
            child.anchoredPosition -= center;
        }
    }


    private void SpawnFloorWithPressPath(GameManager.SpawnFloorPosition spawnFloorPosition)
    {
        floorPosition = currentFloorImageRect.anchoredPosition;
        switch (spawnFloorPosition)
        {
            // y + 
            case GameManager.SpawnFloorPosition.Up:
                InstantiateFloor(new Vector2(floorPosition.x, floorPosition.y + floorSize));
                break;
            // y -
            case GameManager.SpawnFloorPosition.Down:
                InstantiateFloor(new Vector2(floorPosition.x, floorPosition.y - floorSize));
                break;
            // x -
            case GameManager.SpawnFloorPosition.Left:
                InstantiateFloor(new Vector2(floorPosition.x - floorSize, floorPosition.y));
                break;
            // x +
            case GameManager.SpawnFloorPosition.Right:
                InstantiateFloor(new Vector2(floorPosition.x + floorSize, floorPosition.y));
                break;
        }
    }

    private void InstantiateFloor(Vector2 position)
    {
        currentFloorImage = Instantiate(currentFloorImage, displayFloorPathUI);
        DrawFloorColor(ref currentFloorImage);

        currentFloorImageRect = currentFloorImage.GetComponent<RectTransform>();
        currentFloorImageRect.anchoredPosition = position;
    }

    void DrawFloorColor(ref Image image)
    {
        recordFloorColorIndex++;
        if (recordFloorColorIndex > recordFloorColor.Count)
        {
            currentFloorImage.color = Color.white;
        }
        else
        {
            image.color = recordFloorColor[recordFloorColorIndex-1];
        }
    }


    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}