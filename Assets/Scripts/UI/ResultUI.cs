using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI newText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private ParticleSystem confetti;

    const string MAX_SCORE = "MaxScore";
    bool isMaxScore;

    private void Start()
    {
        newText.enabled = false;
        isMaxScore = false;
        GameManager.Instance.OnGameSuccess += GameManager_OnGameSuccess;
        GameManager.Instance.OnGameFailed += GameManager_OnGameFailed;
        HideCurrentGameObject();
    }

    private void GameManager_OnGameSuccess(object sender, EventArgs e)
    {
        resultText.text = "SUCCESS";
        finalScoreText.enabled = false;
        ShowCurrentGameObject();
        SoundManager.Instance.PlaySuccessSound();
        StartCoroutine(ParticleSystemPlayCoroutine());
    }

    private void GameManager_OnGameFailed(object sender, EventArgs e)
    {
        if (GameManager.Instance.GetGameMode() == GameManager.GameMode.Random)
        {
            RandomModeFailed();
        }
        else
        {
            SpecificModeFailed();
        }
    }

    void RandomModeFailed()
    {
        resultText.text = "Top Score";
        finalScoreText.enabled = true;
        finalScoreText.text = GetMaxScore().ToString();
        ShowCurrentGameObject();
        PlaySoundAndEffect();
    }

    int GetMaxScore()
    {
        int maxScore = PlayerPrefs.GetInt(MAX_SCORE);
        if (GameManager.Instance.GetScore() > maxScore)
        {
            newText.enabled = true;
            newText.text = "New!";
            isMaxScore = true;
            PlayerPrefs.SetInt(MAX_SCORE, GameManager.Instance.GetScore());
            return GameManager.Instance.GetScore();
        }

        return maxScore;
    }

    void PlaySoundAndEffect()
    {
        if (isMaxScore)
        {
            StartCoroutine(ParticleSystemPlayCoroutine());
            SoundManager.Instance.PlaySuccessSound();
        }
        else
        {
            SoundManager.Instance.PlayFailSound();
        }
    }

    void SpecificModeFailed()
    {
        resultText.text = "FAILED";
        finalScoreText.enabled = false;
        ShowCurrentGameObject();
        SoundManager.Instance.PlayFailSound();
    }


    private void HideCurrentGameObject()
    {
        gameObject.SetActive(false);
    }

    private void ShowCurrentGameObject()
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