using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject panel;
    public GameObject settingsPanel;
    public Game game;

    private bool isPaused = false;

    private void Start()
    {
        if (panel != null) panel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    private void Update()
    {
        // Toggle pause menu with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        // Don't pause if game is over or already paused
        if ((game != null && game.isGameOver) || isPaused) return;
        
        isPaused = true;
        if (panel != null) panel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Time.timeScale = 0f; // Stop gameplay
    }

    public void Resume()
    {
        isPaused = false;
        if (panel != null) panel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Time.timeScale = 1f; // Resume gameplay
    }

    public void QuitToMainMenu()
    {
        Resume(); // Ensure time scale is reset before loading new scene
        SceneManager.LoadScene("MainMenuScene"); // Adjust scene name as needed
    }

    public void QuitGame()
    {
        Resume(); // Ensure time scale is reset
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    /// <summary>Show settings panel (hides pause menu panel).</summary>
    public void ShowSettings()
    {
        if (panel != null) panel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    /// <summary>Go back from settings to pause menu.</summary>
    public void BackFromSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (panel != null) panel.SetActive(true);
    }

    public bool IsPaused => isPaused;
}
