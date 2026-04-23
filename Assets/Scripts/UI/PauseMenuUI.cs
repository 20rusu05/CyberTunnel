using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private bool isPaused;

    private void Start()
    {
        pausePanel?.SetActive(false);

        resumeButton?.onClick.AddListener(Resume);
        restartButton?.onClick.AddListener(Restart);
        mainMenuButton?.onClick.AddListener(GoToMainMenu);
        quitButton?.onClick.AddListener(QuitGame);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PuzzleUIManager.Instance != null && PuzzleUIManager.Instance.IsPuzzleOpen)
            {
                PuzzleUIManager.Instance.ClosePuzzle();
                return;
            }

            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        pausePanel?.SetActive(true);
        GameManager.Instance?.PauseGame();
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel?.SetActive(false);
        GameManager.Instance?.ResumeGame();
    }

    private void Restart()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance?.LoadScene("GameScene");
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance?.LoadScene("MainMenu");
    }

    private void QuitGame()
    {
        SceneLoader.Instance?.QuitGame();
    }
}
