using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartCountDownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDownText;

    // private const string POPUP = "Popup";
    private static readonly int Popup = Animator.StringToHash("Popup");
    private int priviousNumber;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Hide();
        GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;
    }

    private void GameManager_OnGameStateChange(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountDown())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsCountDown())
        {
            int currentCountDownTimer = Mathf.CeilToInt(GameManager.Instance.GetCountTime());
            countDownText.text = currentCountDownTimer.ToString();

            if (currentCountDownTimer != priviousNumber)
            {
                animator.SetTrigger(Popup);
                priviousNumber = currentCountDownTimer;
                SoundManager.Instance.PlayCountDownSound();
            }
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