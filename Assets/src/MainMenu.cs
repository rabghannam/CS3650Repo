using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main menu controller with buttons for Play, Settings, Credits, and Quit.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    public GameObject upgradesPanel;

    private void Start()
    {
        // Show main menu by default
        ShowMainMenu();
        HideSettings();
        HideCredits();
        HideUpgrades();
    }

    /// <summary>Start the game - loads GameScene.</summary>
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>Show the main menu panel.</summary>
    public void ShowMainMenu()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    /// <summary>Hide the main menu panel.</summary>
    public void HideMainMenu()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
    }

    /// <summary>Show the settings panel.</summary>
    public void ShowSettings()
    {
        HideMainMenu();
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    /// <summary>Hide the settings panel.</summary>
    public void HideSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    /// <summary>Show the credits panel.</summary>
    public void ShowCredits()
    {
        HideMainMenu();
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    /// <summary>Hide the credits panel.</summary>
    public void HideCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    /// <summary>Show the upgrades panel.</summary>
    public void ShowUpgrades()
    {
        HideMainMenu();
        if (upgradesPanel != null) upgradesPanel.SetActive(true);
    }

    /// <summary>Hide the upgrades panel.</summary>
    public void HideUpgrades()
    {
        if (upgradesPanel != null) upgradesPanel.SetActive(false);
    }

    /// <summary>Go back to main menu from any sub-panel.</summary>
    public void BackToMainMenu()
    {
        HideSettings();
        HideCredits();
        HideUpgrades();
        ShowMainMenu();
    }

    /// <summary>Quit the game.</summary>
    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
