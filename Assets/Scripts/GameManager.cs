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

    private float spawnFloorTimer;
    private float spawnFloorTimerMax = 1f;
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

    public event EventHandler OnChangeFloorColor;
    public event EventHandler OnChangeBloomIntensity;
    public event EventHandler OnGameStateChange;
    public event EventHandler OnGameSuccess;
    public event EventHandler OnGameFailed;
    public event EventHandler OnGameStarted;


    public enum SpawnFloorPosition
    {
        Front,
        Back,
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
        // DisplayFloorPath.Instance.recordFloorColor.Add(floorPrefab.GetComponent<MeshRenderer>().material.color);
        DisplayFloorPath.Instance.recordFloorColor.Add(floorPrefab.GetComponent<MeshRenderer>().sharedMaterial.color);
        currentGameObject = floorPrefab;
        floorList.Add(currentGameObject);
        OnGameStarted?.Invoke(this, EventArgs.Empty);


        gameState = GameState.Idle;

        GameStartUI.Instance.OnRandomModeChoose += GameStartUI_OnRandomModeChoose;
        GameStartUI.Instance.OnSpecificyModeChoose += GameStartUI_OnSpecificModeChoose;

        pressData = PressPath.Instance.GetDirection();
        floorNumber = pressData.Length;
        Debug.Log(pressData.Length);
    }

    private void GameStartUI_OnRandomModeChoose(object sender, EventArgs e)
    {
        StartCoroutine(IdleTimeWaitCoroutine(GameMode.Random));
    }

    private void GameStartUI_OnSpecificModeChoose(object sender, EventArgs e)
    {
        StartCoroutine(IdleTimeWaitCoroutine(GameMode.Specific));
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
                // Debug.Log("gameover");
                OnGameStateChange?.Invoke(this, EventArgs.Empty);
                break;
        }
    }


    private void FixedUpdate()
    {
        if (gameState == GameState.PLaying)
        {
            // floorMaterialColor = floorList[0].GetComponent<MeshRenderer>().sharedMaterial.color;
            spawnFloorTimer -= Time.deltaTime;
            if (spawnFloorTimer < 0)
            {
                GameModeChoose(gameMode);
            }
        }
    }

    private void GameModeChoose(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.Random:
                int randomNumber = UnityEngine.Random.Range(0, 3);
                SpawnFloorWithRandom(randomNumber);
                /*spawnFloorTimer = spawnFloorTimerMax;
                FadeAndDestroyFloor();*/
                break;
            case GameMode.Specific:
                currentDirection++;
                if (currentDirection >= pressData.Length)
                {
                    OnGameSuccess?.Invoke(this, EventArgs.Empty);
                    gameState = GameState.GameOver;
                }
                else
                {
                    var x = pressData[currentDirection].direction;
                    SpawnFloorWithPressPath(x);
                    // currentDirection = (currentDirection + 1) % pressData.Length;
                    /*spawnFloorTimer = spawnFloorTimerMax;
                    FadeAndDestroyFloor();*/
                }

                break;
        }

        spawnFloorTimer = spawnFloorTimerMax;
        FadeAndDestroyFloor();
    }

    private void SpawnFloorWithRandom(int randomNumber)
    {
        floorPosition = currentGameObject.transform.position;
        spawnFloorPosition = (SpawnFloorPosition)randomNumber;
        CheckFloorPosition(spawnFloorPosition);
        floorList.Add(currentGameObject);
        if (floorList.Contains(Player.Instance.GetFloorName()))
        {
            successNumber++;
        }
    }

    private void SpawnFloorWithPressPath(SpawnFloorPosition pos)
    {
        floorPosition = currentGameObject.transform.position;
        CheckFloorPosition(pos);
        floorList.Add(currentGameObject);
        if (floorList.Contains(Player.Instance.GetFloorName()))
        {
            floorNumber--;
        }
    }
    
    void CheckFloorPosition(SpawnFloorPosition pos)
    {
        switch (pos)
        {
            // z + 
            case SpawnFloorPosition.Front:
                SetFloorPos(floorPosition.x, floorPosition.z + floorScale);
                break;
            // z-
            case SpawnFloorPosition.Back:
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

        const float duration = 0.7f; //淡出所需时间
        float counter = 0;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0, counter / duration);

            floorMaterialColor = new Color(floorMaterialColor.r, floorMaterialColor.g, floorMaterialColor.b, alpha);
            floorMaterial.color = floorMaterialColor;

            yield return null;
        }

        floorList.RemoveAt(0);
        // Destroy(floor);
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

    public int GetSuccessedNumber()
    {
        if (gameMode == GameMode.Random)
        {
            return successNumber;
        }

        return floorNumber;
    }

    private void SetGameMode(GameMode gameMode)
    {
        this.gameMode = gameMode;
    }

    public GameMode GetGameMode()
    {
        return gameMode;
    }


    private IEnumerator IdleTimeWaitCoroutine(GameMode gameMode)
    {
        yield return new WaitForSeconds(2f);
        gameState = GameState.StartCountDown;
        SetGameMode(gameMode);
        OnGameStateChange?.Invoke(this, EventArgs.Empty);
    }
}