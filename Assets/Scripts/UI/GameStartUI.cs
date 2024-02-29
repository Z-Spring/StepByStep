using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameStartUI : MonoBehaviour
{
    public static GameStartUI Instance { get; private set; }
    public event Action<GameManager.GameMode> OnGameModeChoose;

    [SerializeField] Button randomBtn;
    [SerializeField] Button specificBtn;
    GameManager.GameMode gameMode;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Show();
        randomBtn.onClick.AddListener(ButtonClick);
        specificBtn.onClick.AddListener(ButtonClick);
    }

    void ButtonClick()
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
        SoundManager.Instance.PlayClickButtonSound();
        if (selectedObject == randomBtn.gameObject)
        {
            gameMode = GameManager.GameMode.Random;
        }
        else if (selectedObject == specificBtn.gameObject)
        {
            gameMode = GameManager.GameMode.Specific;
        }

        OnGameModeChoose?.Invoke(gameMode);
        Hide();
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