using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI controller for the Settings Panel.
/// Handles volume slider, fullscreen toggle, and resolution dropdown.
/// Works with SettingsManager to save/load settings.
/// </summary>
public class SettingsPanel : MonoBehaviour
{
    [Header("UI References")]
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Button applyButton;
    public Button backButton;

    // Reference to the menu that opened this panel (for back navigation)
    public MainMenu mainMenu;
    public PauseMenu pauseMenu;

    private void OnEnable()
    {
        // Initialize UI with current settings
        InitializeUI();
    }

    /// <summary>Initialize UI elements with current settings from SettingsManager.</summary>
    private void InitializeUI()
    {
        SettingsManager settings = SettingsManager.Instance;
        if (settings == null)
        {
            Debug.LogWarning("SettingsManager instance not found!");
            return;
        }

        // Volume slider
        if (volumeSlider != null)
        {
            volumeSlider.value = settings.Volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // Fullscreen toggle
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = settings.Fullscreen;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        }

        // Resolution dropdown
        if (resolutionDropdown != null)
        {
            PopulateResolutionDropdown();
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }

        // Apply button
        if (applyButton != null)
        {
            applyButton.onClick.AddListener(OnApplyClicked);
        }

        // Back button
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    /// <summary>Populate the resolution dropdown with available resolutions.</summary>
    private void PopulateResolutionDropdown()
    {
        SettingsManager settings = SettingsManager.Instance;
        if (settings == null || resolutionDropdown == null) return;

        Resolution[] resolutions = settings.GetAvailableResolutions();
        resolutionDropdown.ClearOptions();

        foreach (Resolution res in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData($"{res.width}x{res.height}"));
        }

        // Select current resolution
        int currentIndex = settings.GetCurrentResolutionIndex();
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // Event handlers
    private void OnVolumeChanged(float value)
    {
        SettingsManager settings = SettingsManager.Instance;
        if (settings != null)
        {
            settings.Volume = value;
        }
    }

    private void OnFullscreenChanged(bool value)
    {
        SettingsManager settings = SettingsManager.Instance;
        if (settings != null)
        {
            settings.Fullscreen = value;
        }
    }

    private void OnResolutionChanged(int index)
    {
        SettingsManager settings = SettingsManager.Instance;
        if (settings != null && index >= 0 && index < Screen.resolutions.Length)
        {
            Resolution res = Screen.resolutions[index];
            settings.ResolutionWidth = res.width;
            settings.ResolutionHeight = res.height;
        }
    }

    private void OnApplyClicked()
    {
        SettingsManager settings = SettingsManager.Instance;
        if (settings != null)
        {
            settings.SaveSettings();
            Debug.Log("Settings applied and saved!");
        }
    }

    private void OnBackClicked()
    {
        SettingsManager settings = SettingsManager.Instance;
        if (settings != null)
        {
            // Revert to saved settings (cancel changes)
            settings.LoadSettings();
        }

        // Return to the menu that opened this panel
        if (mainMenu != null)
        {
            mainMenu.BackToMainMenu();
        }
        else if (pauseMenu != null)
        {
            pauseMenu.BackFromSettings();
        }
        else
        {
            // Fallback: just hide this panel
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        // Remove listeners to prevent memory leaks
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenChanged);
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
        if (applyButton != null)
            applyButton.onClick.RemoveListener(OnApplyClicked);
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackClicked);
    }
}
