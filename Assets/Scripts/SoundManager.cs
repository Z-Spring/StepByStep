using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClipsSO audioClipsSO;
    [SerializeField] private Transform player;

    private void Awake()
    {
        Instance = this;
    }

    private void PlaySound(AudioClip[] audioClips, Vector3 position, int volume = 1)
    {
        int rangeNumber = Random.Range(0, audioClips.Length);
        AudioSource.PlayClipAtPoint(audioClips[rangeNumber], position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, int volume = 1)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }

    public void PlayCountDownSound()
    {
        PlaySound(audioClipsSO.countDownClips, Vector3.zero);
    }

    public void PlaySuccessSound()
    {
        PlaySound(audioClipsSO.successClip, player.position);
    }

    public void PlayFailSound()
    {
        PlaySound(audioClipsSO.failClip, player.position);
    }

    public void PlayClickButtonSound()
    {
        PlaySound(audioClipsSO.clickButtonClip, Vector3.zero);
    }
}