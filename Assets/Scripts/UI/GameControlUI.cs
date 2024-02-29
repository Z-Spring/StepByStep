using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControlUI : MonoBehaviour
{
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button quitBtn;
    
    private void Start()
    {
        replayBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
            SoundManager.Instance.PlayClickButtonSound();

        });

        quitBtn.onClick.AddListener(() =>
        {
            Application.Quit(0);
            SoundManager.Instance.PlayClickButtonSound();

        });
    }


}