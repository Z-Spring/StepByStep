using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private ParticleSystem confetti;

    private void Start()
    {
        GameManager.Instance.OnGameSuccess += GameManager_OnGameSuccess;
        GameManager.Instance.OnGameFailed += GameManager_OnGameFailed;
        Hide();
    }

    private void GameManager_OnGameSuccess(object sender, EventArgs e)
    {
        resultText.text = "SUCCESS";
        finalScoreText.enabled = false;
        Show();
        SoundManager.Instance.PlaySuccessSound();
        StartCoroutine(ParticleSystemPlayCoroutine());
    }

    private void GameManager_OnGameFailed(object sender, EventArgs e)
    {
        if (GameManager.Instance.GetGameMode() == GameManager.GameMode.Random)
        {
            finalScoreText.text = GameManager.Instance.GetSuccessedNumber().ToString();
            resultText.text = "FAILED";
            Show();
            SoundManager.Instance.PlayFailSound();

        }
        else
        {
            resultText.text = "FAILED";
            finalScoreText.enabled = false;
            Show();
            SoundManager.Instance.PlayFailSound();

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

//Coroutine
    private IEnumerator ParticleSystemPlayCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        confetti.Play();
    }
}