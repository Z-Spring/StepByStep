using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Background : MonoBehaviour
{
    private float changeBackgroundTimer;
    private float changeBackgroundTimerMax = 20f;
    [SerializeField] private List<Color> backgroundColorList;
    private int backgroundColorListLength;

    private void Awake()
    {
        backgroundColorListLength = backgroundColorList.Count;
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying())
        {
            changeBackgroundTimer += Time.deltaTime;
            if (changeBackgroundTimer > changeBackgroundTimerMax)
            {
                ChangeBackground();
                changeBackgroundTimer = 0;
            }
        }
    }


    private void ChangeBackground()
    {
        int randomColorNumber = Random.Range(0, backgroundColorListLength);
        Camera.main.backgroundColor = backgroundColorList[randomColorNumber];
    }
}