using UnityEngine;

/// <summary>
/// Manages game settings (volume, fullscreen, resolution) and persists them using PlayerPrefs.
/// Singleton that auto-creates itself if needed - works from any scene.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    // Volume (0-1)
    private const string VOLUME_KEY = "Settings_Volume";
    private float _volume = 1.0f;
    
    // Fullscreen
    private const string FULLSCREEN_KEY = "Settings_Fullscreen";
    private bool _fullscreen = true;
    
    // Resolution
    private const string RESOLUTION_WIDTH_KEY = "Settings_ResolutionWidth";
    private const string RESOLUTION_HEIGHT_KEY = "Settings_ResolutionHeight";
    private int _resolutionWidth = 1920;
    private int _resolutionHeight = 1080;

    // Singleton instance
    private static SettingsManager _instance;
    
    public static SettingsManager Instance
    {
        get
        {
            // Try to find existing instance
            if (_instance == null)
            {
                _instance = FindObjectOfType<SettingsManager>();
            }
            
            // If still null, create one automatically
            if (_instance == null)
            {
                GameObject obj = new GameObject("SettingsManager");
                _instance = obj.AddComponent<SettingsManager>();
            }
            
            return _instance;
        }
    }

    private void Awake()
    {
        // Singleton pattern - only one instance persists
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
        
        LoadSettings();
    }

    /// <summary>Load settings from PlayerPrefs.</summary>
    public void LoadSettings()
    {
        _volume = PlayerPrefs.GetFloat(VOLUME_KEY, 1.0f);
        _fullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;
        _resolutionWidth = PlayerPrefs.GetInt(RESOLUTION_WIDTH_KEY, 1920);
        _resolutionHeight = PlayerPrefs.GetInt(RESOLUTION_HEIGHT_KEY, 1080);
        
        ApplySettings();
    }

    /// <summary>Save settings to PlayerPrefs.</summary>
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(VOLUME_KEY, _volume);
        PlayerPrefs.SetInt(FULLSCREEN_KEY, _fullscreen ? 1 : 0);
        PlayerPrefs.SetInt(RESOLUTION_WIDTH_KEY, _resolutionWidth);
        PlayerPrefs.SetInt(RESOLUTION_HEIGHT_KEY, _resolutionHeight);
        PlayerPrefs.Save();
        
        ApplySettings();
    }

    /// <summary>Apply current settings to the game.</summary>
    private void ApplySettings()
    {
        // Apply volume to AudioListener
        AudioListener.volume = _volume;
        
        // Apply fullscreen and resolution
        Screen.SetResolution(_resolutionWidth, _resolutionHeight, _fullscreen);
    }

    // Public properties with setters that trigger save
    public float Volume
    {
        get => _volume;
        set
        {
            _volume = Mathf.Clamp01(value);
            AudioListener.volume = _volume;
        }
    }

    public bool Fullscreen
    {
        get => _fullscreen;
        set
        {
            _fullscreen = value;
        }
    }

    public int ResolutionWidth
    {
        get => _resolutionWidth;
        set => _resolutionWidth = value;
    }

    public int ResolutionHeight
    {
        get => _resolutionHeight;
        set => _resolutionHeight = value;
    }

    /// <summary>Set resolution from a string (e.g., "1920x1080").</summary>
    public void SetResolution(string resolutionString)
    {
        string[] parts = resolutionString.Split('x');
        if (parts.Length == 2)
        {
            if (int.TryParse(parts[0], out int width) && int.TryParse(parts[1], out int height))
            {
                _resolutionWidth = width;
                _resolutionHeight = height;
            }
        }
    }

    /// <summary>Get current resolution as string.</summary>
    public string GetResolutionString() => $"{_resolutionWidth}x{_resolutionHeight}";

    /// <summary>Get available screen resolutions.</summary>
    public Resolution[] GetAvailableResolutions() => Screen.resolutions;

    /// <summary>Get current screen resolution index from available resolutions.</summary>
    public int GetCurrentResolutionIndex()
    {
        Resolution[] resolutions = Screen.resolutions;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == _resolutionWidth && 
                resolutions[i].height == _resolutionHeight)
            {
                return i;
            }
        }
        return 0;
    }
}
