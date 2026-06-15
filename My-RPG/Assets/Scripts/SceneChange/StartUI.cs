using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    [Header("按钮引用")]
    public Button startButton;

    [Header("场景名")]
    public string gameSceneName = "zktScene";

    private void Start()
    {
        startButton.onClick.AddListener(OnStartGame);
    }

    private void OnStartGame()
    {
        MusicManager.Instance?.PlayClick();
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
