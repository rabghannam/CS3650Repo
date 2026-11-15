using UnityEngine;

/// <summary>
/// Manages persistent player progression data (money, upgrade levels) between runs.
/// Singleton that persists across scenes using DontDestroyOnLoad.
/// Uses PlayerPrefs for persistence.
/// </summary>
public class ProgressManager : MonoBehaviour
{
    // Player currency
    private const string MONEY_KEY = "Progress_Money";
    private int _money = 0;

    // Upgrade levels - keys match GDD upgrade names
    private const string MAX_HEALTH_LEVEL_KEY = "Progress_MaxHealthLevel";
    private int _maxHealthLevel = 0;

    private const string FIRE_RATE_LEVEL_KEY = "Progress_FireRateLevel";
    private int _fireRateLevel = 0;

    private const string BASE_DAMAGE_LEVEL_KEY = "Progress_BaseDamageLevel";
    private int _baseDamageLevel = 0;

    // Singleton instance
    private static ProgressManager _instance;

    public static ProgressManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ProgressManager>();
            }

            if (_instance == null)
            {
                GameObject obj = new GameObject("ProgressManager");
                _instance = obj.AddComponent<ProgressManager>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        LoadProgress();
    }

    /// <summary>Load progression data from PlayerPrefs.</summary>
    public void LoadProgress()
    {
        _money = PlayerPrefs.GetInt(MONEY_KEY, 0);
        _maxHealthLevel = PlayerPrefs.GetInt(MAX_HEALTH_LEVEL_KEY, 0);
        _fireRateLevel = PlayerPrefs.GetInt(FIRE_RATE_LEVEL_KEY, 0);
        _baseDamageLevel = PlayerPrefs.GetInt(BASE_DAMAGE_LEVEL_KEY, 0);
    }

    /// <summary>Save progression data to PlayerPrefs.</summary>
    public void SaveProgress()
    {
        PlayerPrefs.SetInt(MONEY_KEY, _money);
        PlayerPrefs.SetInt(MAX_HEALTH_LEVEL_KEY, _maxHealthLevel);
        PlayerPrefs.SetInt(FIRE_RATE_LEVEL_KEY, _fireRateLevel);
        PlayerPrefs.SetInt(BASE_DAMAGE_LEVEL_KEY, _baseDamageLevel);
        PlayerPrefs.Save();
    }

    // Public properties
    public int Money
    {
        get => _money;
        set
        {
            _money = Mathf.Max(0, value);
            SaveProgress();
        }
    }

    public int MaxHealthLevel
    {
        get => _maxHealthLevel;
        set
        {
            _maxHealthLevel = Mathf.Clamp(value, 0, 5); // Max level 5 from GDD
            SaveProgress();
        }
    }

    public int FireRateLevel
    {
        get => _fireRateLevel;
        set
        {
            _fireRateLevel = Mathf.Clamp(value, 0, 5); // Max level 5 from GDD
            SaveProgress();
        }
    }

    public int BaseDamageLevel
    {
        get => _baseDamageLevel;
        set
        {
            _baseDamageLevel = Mathf.Clamp(value, 0, 20); // Max level 20 from GDD
            SaveProgress();
        }
    }

    /// <summary>Add money to player's balance.</summary>
    public void AddMoney(int amount)
    {
        Money += amount;
    }

    /// <summary>Check if player can afford an upgrade.</summary>
    public bool CanAfford(int cost)
    {
        return _money >= cost;
    }

    /// <summary>Get cost for next level of Max Health upgrade.</summary>
    public int GetMaxHealthCost()
    {
        // From GDD: Cost is 100 per level (100 * (level + 1)?)
        // Actually GDD says cost is 100 for the upgrade, max level 5
        // So it's a flat cost of 100 per level
        return 100;
    }

    /// <summary>Get cost for next level of Fire Rate upgrade.</summary>
    public int GetFireRateCost()
    {
        // From GDD: Cost is 150 per level
        return 150;
    }

    /// <summary>Get cost for next level of Base Damage upgrade.</summary>
    public int GetBaseDamageCost()
    {
        // From GDD: Cost is 200 per level
        return 200;
    }

    /// <summary>Get the actual max health value based on upgrade level.</summary>
    public int GetActualMaxHealth()
    {
        // Base health is 50, +10 per level
        return Player.BASE_HEALTH + (_maxHealthLevel * 10);
    }

    /// <summary>Get the actual fire rate value based on upgrade level.</summary>
    public float GetActualFireRate()
    {
        // Base fire rate is 5, +1 per level
        return Player.FIRE_RATE + _fireRateLevel;
    }

    /// <summary>Get the actual base damage value based on upgrade level.</summary>
    public int GetActualBaseDamage()
    {
        // Base damage is 1, +1 per level
        return 1 + _baseDamageLevel;
    }

    /// <summary>Reset all progression (for testing/development).</summary>
    public void ResetProgress()
    {
        _money = 0;
        _maxHealthLevel = 0;
        _fireRateLevel = 0;
        _baseDamageLevel = 0;
        SaveProgress();
    }
}
