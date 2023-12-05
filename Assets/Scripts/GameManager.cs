using System;
using System.Collections;
using System.Collections.Generic;
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

        // currentGameObject = floorSO.floor;
        DisplayFloorPath.Instance.recordFloorColor.Add(floorPrefab.GetComponent<MeshRenderer>().material.color);
        currentGameObject = floorPrefab;
        floorList.Add(currentGameObject);
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        gameState = GameState.Idle;
        
        GameStartUI.Instance.OnRandomModeChoose += GameStartUI_OnRandomModeChoose;
        GameStartUI.Instance.OnSpecificyModeChoose += GameStartUI_OnSpecificModeChoose;
        
        pressData = PressPath.Instance.GetDirection();
        floorNumber = pressData.Length;
        Debug.Log(pressData.Length);
    }

    private void GameStartUI_OnRandomModeChoose(object sender, EventArgs e)
    {
        // gameState = GameState.StartCountDown;
        // SetGameMode(GameMode.Random);
        // OnGameStateChange?.Invoke(this, EventArgs.Empty);
        StartCoroutine(IdleTimeWaitCoroutine(GameMode.Random));
    }

    private void GameStartUI_OnSpecificModeChoose(object sender, EventArgs e)
    {
        // gameState = GameState.StartCountDown;
        // SetGameMode(GameMode.Specific);
        // OnGameStateChange?.Invoke(this, EventArgs.Empty);
        StartCoroutine(IdleTimeWaitCoroutine(GameMode.Specific));

    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.Idle:
                // gameState = GameState.StartCountDown;
                // OnGameStateChange?.Invoke(this, EventArgs.Empty);
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
            floorMaterialColor = floorList[0].GetComponent<MeshRenderer>().material.color;
            spawnFloorTimer -= Time.deltaTime;
            if (spawnFloorTimer < 0)
            {
                GameModeChoose(gameMode);
            }
        }
    }


    private void SpawnFloorWithRandom(int randomNumber)
    {
        floorPosition = currentGameObject.transform.position;
        spawnFloorPosition = (SpawnFloorPosition)randomNumber;

        switch (spawnFloorPosition)
        {
            // z + 
            case SpawnFloorPosition.Front:
                currentGameObject = Instantiate(currentGameObject,
                    new Vector3(floorPosition.x, 0, floorPosition.z + floorScale),
                    Quaternion.identity);
                OnChangeFloorColor?.Invoke(currentGameObject, EventArgs.Empty);
                OnChangeBloomIntensity?.Invoke(currentGameObject, EventArgs.Empty);
                // currentGameObject
                break;
            // z-
            case SpawnFloorPosition.Back:
                currentGameObject = Instantiate(currentGameObject,
                    new Vector3(floorPosition.x, 0, floorPosition.z - floorScale),
                    Quaternion.identity);
                OnChangeFloorColor?.Invoke(currentGameObject, EventArgs.Empty);
                OnChangeBloomIntensity?.Invoke(currentGameObject, EventArgs.Empty);

                break;
            // x -
            case SpawnFloorPosition.Left:
                currentGameObject = Instantiate(currentGameObject,
                    new Vector3(floorPosition.x - floorScale, 0, floorPosition.z),
                    Quaternion.identity);

                OnChangeFloorColor?.Invoke(currentGameObject, EventArgs.Empty);
                OnChangeBloomIntensity?.Invoke(currentGameObject, EventArgs.Empty);

                break;
            // x +
            case SpawnFloorPosition.Right:
                currentGameObject = Instantiate(currentGameObject,
                    new Vector3(floorPosition.x + floorScale, 0, floorPosition.z), Quaternion.identity);

                OnChangeFloorColor?.Invoke(currentGameObject, EventArgs.Empty);
                OnChangeBloomIntensity?.Invoke(currentGameObject, EventArgs.Empty);

                break;
            default:
                currentGameObject = Instantiate(currentGameObject,
                    new Vector3(floorPosition.x, 0, floorPosition.z), Quaternion.identity);
                break;
        }

        floorList.Add(currentGameObject);
        if (floorList.Contains(Player.Instance.GetFloorName()))
        {
            successNumber++;
        }
    }

    private void SpawnFloorWithPressPath(SpawnFloorPosition spawnFloorPosition)
    {
        floorPosition = currentGameObject.transform.position;
        // spawnFloorPosition = (SpawnFloorPosition)randomNumber;

        switch (spawnFloorPosition)
        {
            // z + 
            case SpawnFloorPosition.Front:
                currentGameObject = Instantiate(currentGameObject,
                    new Vector3(floorPosition.x, 0, floorPosition.z + floorScale),
                    Quaternion.identity);

                OnChangeFloorColor?.Invoke(currentGameObject, EventArgs.Empty);
                OnChangeBloomIntensity?.Invoke(currentGameObject, EventArgs.Empty);
                // currentGameObject
                break;
            // z-
            case SpawnFloorPosition.Back:
                currentGameObject = Instantiate(currentGameObject,
                    new Vector3(floorPosition.x, 0, floorPosition.z - floorScale),
                    Quaternion.identity);

                OnChangeFloorColor?.Invoke(currentGameObject, EventArgs.Empty);
                OnChangeBloomIntensity?.Invoke(currentGameObject, EventArgs.Empty);

                break;
            // x -
            case SpawnFloorPosition.Left:
                currentGameObject = Instantiate(currentGameObject,
                    new Vector3(floorPosition.x - floorScale, 0, floorPosition.z),
                    Quaternion.identity);

                OnChangeFloorColor?.Invoke(currentGameObject, EventArgs.Empty);
                OnChangeBloomIntensity?.Invoke(currentGameObject, EventArgs.Empty);

                break;
            // x +
            case SpawnFloorPosition.Right:
                currentGameObject = Instantiate(currentGameObject,
                    new Vector3(floorPosition.x + floorScale, 0, floorPosition.z), Quaternion.identity);

                OnChangeFloorColor?.Invoke(currentGameObject, EventArgs.Empty);
                OnChangeBloomIntensity?.Invoke(currentGameObject, EventArgs.Empty);

                break;
            default:
                currentGameObject = Instantiate(currentGameObject,
                    new Vector3(floorPosition.x, 0, floorPosition.z), Quaternion.identity);
                break;
        }

        floorList.Add(currentGameObject);
        if (floorList.Contains(Player.Instance.GetFloorName()))
        {
            // floorNumber++;
            // pressData.Length
            floorNumber--;
        }
    }

    private void FadeAndDestroyFloor()
    {
        StartCoroutine(FadeOut(floorList[0]));
    }

    private IEnumerator FadeOut(GameObject floor)
    {
        Material floorMaterial = floor.GetComponent<MeshRenderer>().material;
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
        Destroy(floor);
    }

    //todo； 走完后，产生走路的轨迹图或路线图 || GAMEOVER后将路线图展示出来，走过的floor点亮，反之变暗
    // private void DisPlayPath()
    // {
    //     
    // }

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

    private void GameModeChoose(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.Random:
                int randomNumber = UnityEngine.Random.Range(0, 3);
                SpawnFloorWithRandom(randomNumber);
                spawnFloorTimer = spawnFloorTimerMax;
                FadeAndDestroyFloor();
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
                    spawnFloorTimer = spawnFloorTimerMax;
                    FadeAndDestroyFloor();
                }

                break;
        }
    }

    private IEnumerator IdleTimeWaitCoroutine(GameMode gameMode)
    {
        yield return new WaitForSeconds(2f);
        gameState = GameState.StartCountDown;
        SetGameMode(gameMode);
        OnGameStateChange?.Invoke(this, EventArgs.Empty);
    }
}