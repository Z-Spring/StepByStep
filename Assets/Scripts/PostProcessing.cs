using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessing : MonoBehaviour
{
    public static PostProcessing Instance { get; private set; }
    [SerializeField] private float bloomIntensity = 2.5f;
    [SerializeField] private float bloomDuration = 0.3f;
    [SerializeField] private float bloomNormalIntensity = 0.7f;
    private Volume volume;
    WaitForSeconds bloomWaitForSeconds;
    private void Awake()
    {
        Instance = this;
        volume = GetComponent<Volume>();
        bloomWaitForSeconds = new WaitForSeconds(bloomDuration);
    }

    private void Start()
    {
        GameManager.Instance.OnChangeBloomIntensity += GameManager_OnChangeBloomIntensity;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnChangeBloomIntensity -= GameManager_OnChangeBloomIntensity;
    }

    private void GameManager_OnChangeBloomIntensity(object sender, EventArgs e)
    {
        StartCoroutine(ChangeBloomIntensityCoroutine());
    }
    
    private IEnumerator ChangeBloomIntensityCoroutine()
    {
        ChangeBloomIntensity(bloomIntensity);
        yield return bloomWaitForSeconds;
        ChangeBloomIntensity(bloomNormalIntensity);
    }

    private void ChangeBloomIntensity(float intensity)
    {
        if (volume.profile.TryGet<Bloom>(out var bloom))
        {
            bloom.intensity.value = intensity;
        }
    }
}