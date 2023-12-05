using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SocreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDownText;

    // private const string POPUP = "Popup";
    private static readonly int Popup = Animator.StringToHash("Popup");
    private Animator animator;
    private float countDownNumber;
    private int priviousNumber;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        // Hide();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;
        Hide();
    }

    private void GameManager_OnGameStateChange(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsPlaying())
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
        
        countDownText.text = GameManager.Instance.GetSuccessedNumber().ToString();
        if (GameManager.Instance.GetSuccessedNumber() != priviousNumber)
        {
            animator.SetTrigger(Popup);
            priviousNumber = GameManager.Instance.GetSuccessedNumber();
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
