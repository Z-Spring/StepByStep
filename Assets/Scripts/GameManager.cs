using System;
using System.Collections;
using System.Collections.Generic;
using Pool;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private FloorSO floorSO;
    [SerializeField] private Image floorImage;
    [SerializeField] float spawnFloorTimerMax = 1f;
    [SerializeField] float fadeOutDuration = 0.7f;
    [SerializeField] float waitTimeBeforeCountDown = 2f;

    private float spawnFloorTimer;
    private Vector3 floorPosition;
    private float floorScale = 0.6f;
    private GameObject currentGameObject;
    private List<GameObject> floorList;
    private float escapeTimer;
    private Color floorMaterialColor;
    private float countDownTimer = 3f;
    private int floorNumber;
    private int successNumber;
    private int currentDirection = -1;
    private PressPath.PressData[] pressData;
    private Action spawnFloorMethod;

    public event EventHandler OnChangeFloorColor;
    public event EventHandler OnChangeBloomIntensity;
    public event EventHandler OnGameStateChange;
    public event EventHandler OnGameSuccess;
    public event EventHandler OnGameFailed;
    public event EventHandler OnGameStarted;


    public enum SpawnFloorPosition
    {
        Up,
        Down,
        Left,
        Right
    }

    private SpawnFloorPosition spawnFloorPosition;

    private enum GameState
    {
        Idle,
        StartCountDown,
        PLaying,
        GameOver
    }

    private GameState gameState;

    public enum GameMode
    {
        Random,
        Specific
    }

    private GameMode gameMode;

    private void Awake()
    {
        Instance = this;
        floorList = new List<GameObject>();
    }

    private void Start()
    {
        OnChangeFloorColor?.Invoke(floorPrefab, EventArgs.Empty);
        currentGameObject = floorPrefab;
        floorList.Add(currentGameObject);
        OnGameStarted?.Invoke(this, EventArgs.Empty);

        gameState = GameState.Idle;

        GameStartUI.Instance.OnGameModeChoose += GameStartUI_OnGameModeChoose;

        pressData = PressPath.Instance.GetDirection();
        floorNumber = pressData.Length;
        Debug.Log(pressData.Length);
    }

    private void GameStartUI_OnGameModeChoose(GameMode mode)
    {
        StartCoroutine(IdleTimeWaitCoroutine(mode));
    }

    private IEnumerator IdleTimeWaitCoroutine(GameMode mode)
    {
        yield return new WaitForSeconds(waitTimeBeforeCountDown);
        gameState = GameState.StartCountDown;
        SetGameMode(mode);
        DetermineGameMode();
        OnGameStateChange?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.Idle:
                break;
            case GameState.StartCountDown:
                countDownTimer -= Time.deltaTime;
                if (countDownTimer <= 0)
                {
                    gameState = GameState.PLaying;
                    OnGameStateChange?.Invoke(this, EventArgs.Empty);
                }

                break;
            case GameState.PLaying:
                if (Player.Instance.IsGrounded())
                {
                    gameState = GameState.PLaying;
                    OnGameStateChange?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    gameState = GameState.GameOver;
                    OnGameFailed?.Invoke(this, EventArgs.Empty);
                }

                break;
            case GameState.GameOver:
                OnGameStateChange?.Invoke(this, EventArgs.Empty);
                break;
        }
    }


    private void FixedUpdate()
    {
        if (gameState == GameState.PLaying)
        {
            spawnFloorTimer -= Time.deltaTime;
            if (spawnFloorTimer < 0)
            {
                spawnFloorMethod();
                ResetSpawnFloorTimer();
                FadeAndDestroyFloor();
            }
        }
    }

    private void DetermineGameMode()
    {
        switch (gameMode)
        {
            case GameMode.Random:
                spawnFloorMethod = SpawnFloorWithRandom;
                break;
            case GameMode.Specific:
                spawnFloorMethod = SpawnFloorWithPressPath;
                break;
        }
    }


    private void SpawnFloorWithRandom()
    {
        floorPosition = currentGameObject.transform.position;

        int randomNumber = UnityEngine.Random.Range(0, 4);
        spawnFloorPosition = (SpawnFloorPosition)randomNumber;
        CheckFloorPosition(spawnFloorPosition);
        floorList.Add(currentGameObject);
        if (floorList.Contains(Player.Instance.GetFloorName()))
        {
            successNumber++;
        }
    }

    private void SpawnFloorWithPressPath()
    {
        currentDirection++;
        if (currentDirection >= pressData.Length)
        {
            OnGameSuccess?.Invoke(this, EventArgs.Empty);
            gameState = GameState.GameOver;
        }
        else
        {
            var pos = pressData[currentDirection].direction;
            floorPosition = currentGameObject.transform.position;
            CheckFloorPosition(pos);
            floorList.Add(currentGameObject);
            if (floorList.Contains(Player.Instance.GetFloorName()))
            {
                floorNumber--;
            }
        }
    }

    void ResetSpawnFloorTimer()
    {
        spawnFloorTimer = spawnFloorTimerMax;
    }

    void CheckFloorPosition(SpawnFloorPosition pos)
    {
        switch (pos)
        {
            // z + 
            case SpawnFloorPosition.Up:
                SetFloorPos(floorPosition.x, floorPosition.z + floorScale);
                break;
            // z-
            case SpawnFloorPosition.Down:
                SetFloorPos(floorPosition.x, floorPosition.z - floorScale);
                break;
            // x -
            case SpawnFloorPosition.Left:
                SetFloorPos(floorPosition.x - floorScale, floorPosition.z);
                break;
            // x +
            case SpawnFloorPosition.Right:
                SetFloorPos(floorPosition.x + floorScale, floorPosition.z);
                break;
            default:
                currentGameObject = Instantiate(currentGameObject,
                    new Vector3(floorPosition.x, 0, floorPosition.z), Quaternion.identity);
                break;
        }
    }

    void SetFloorPos(float posX, float posZ)
    {
        currentGameObject = FloorPool.Instance.GetFloorFromPool();
        currentGameObject.transform.SetPositionAndRotation(
            new Vector3(posX, 0, posZ),
            Quaternion.identity);
        OnChangeFloorColor?.Invoke(currentGameObject, EventArgs.Empty);
        OnChangeBloomIntensity?.Invoke(currentGameObject, EventArgs.Empty);
    }

    private void FadeAndDestroyFloor()
    {
        StartCoroutine(FadeOut(floorList[0]));
    }

    private IEnumerator FadeOut(GameObject floor)
    {
        Material floorMaterial = floor.GetComponent<MeshRenderer>().sharedMaterial;
        Color floorMaterialColor = floorMaterial.color;
        float startAlpha = floorMaterialColor.a;
        float counter = 0;
        while (counter < fadeOutDuration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0, counter / fadeOutDuration);

            floorMaterialColor = new Color(floorMaterialColor.r, floorMaterialColor.g, floorMaterialColor.b, alpha);
            floorMaterial.color = floorMaterialColor;

            yield return null;
        }

        floorList.RemoveAt(0);
        FloorPool.Instance.ReturnFloor(floor);
    }


    public bool IsGameOver()
    {
        return gameState == GameState.GameOver;
    }

    public bool IsPlaying()
    {
        return gameState == GameState.PLaying;
    }

    public bool IsCountDown()
    {
        return gameState == GameState.StartCountDown;
    }

    public float GetCountTime()
    {
        return countDownTimer;
    }

    public int GetScore()
    {
        if (gameMode == GameMode.Random)
        {
            return successNumber;
        }

        return floorNumber;
    }

    void SetGameMode(GameMode gameMode)
    {
        this.gameMode = gameMode;
    }

    public GameMode GetGameMode()
    {
        return gameMode;
    }
}