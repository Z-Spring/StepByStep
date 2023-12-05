using System;
using UnityEngine;
using UnityEngine.UI;

public class GameStartUI : MonoBehaviour
{
    public static GameStartUI Instance { get; private set; }

    [SerializeField] private Button randomBtn;
    [SerializeField] private Button sprcificBtn;
    public event EventHandler OnRandomModeChoose;
    public event EventHandler OnSpecificyModeChoose;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Show();
        randomBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayClickButtonSound();
            OnRandomModeChoose?.Invoke(this, EventArgs.Empty);
            Hide();
        });

        sprcificBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayClickButtonSound();
            OnSpecificyModeChoose?.Invoke(this, EventArgs.Empty);
            Hide();
        });
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