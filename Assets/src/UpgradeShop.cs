using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the upgrade shop UI between runs.
/// Displays all available upgrades with icons, current level, cost, and player's money balance.
/// </summary>
public class UpgradeShop : MonoBehaviour
{
    [Header("UI References")]
    public Text moneyText;
    
    [Header("Max Health Upgrade")]
    public Image maxHealthIcon;
    public Text maxHealthLevelText;
    public Text maxHealthCostText;
    public Button maxHealthButton;
    public GameObject maxHealthMaxedIndicator;

    [Header("Fire Rate Upgrade")]
    public Image fireRateIcon;
    public Text fireRateLevelText;
    public Text fireRateCostText;
    public Button fireRateButton;
    public GameObject fireRateMaxedIndicator;

    [Header("Base Damage Upgrade")]
    public Image baseDamageIcon;
    public Text baseDamageLevelText;
    public Text baseDamageCostText;
    public Button baseDamageButton;
    public GameObject baseDamageMaxedIndicator;

    private ProgressManager _progressManager;

    private void Start()
    {
        // Get or create the ProgressManager
        _progressManager = ProgressManager.Instance;
        
        // Note: Button click events are handled by individual UpgradeButton scripts
        // UpgradeShop only manages the display, not the click events
        
        // Load and display initial state
        RefreshShop();
    }

    private void OnEnable()
    {
        // Refresh whenever the shop panel is shown
        if (_progressManager == null)
            _progressManager = ProgressManager.Instance;
        
        RefreshShop();
    }

    /// <summary>Refresh all shop UI elements with current data.</summary>
    public void RefreshShop()
    {
        if (_progressManager == null)
            _progressManager = ProgressManager.Instance;

        // Update money display
        if (moneyText != null)
            moneyText.text = "Money: " + _progressManager.Money;

        // Update Max Health upgrade display
        UpdateMaxHealthDisplay();
        
        // Update Fire Rate upgrade display
        UpdateFireRateDisplay();
        
        // Update Base Damage upgrade display
        UpdateBaseDamageDisplay();
    }

    private void UpdateMaxHealthDisplay()
    {
        if (maxHealthLevelText != null)
            maxHealthLevelText.text = "Level: " + _progressManager.MaxHealthLevel + " / 5";
        
        if (maxHealthCostText != null)
            maxHealthCostText.text = "Cost: " + _progressManager.GetMaxHealthCost();
        
        // Check if maxed out
        bool isMaxed = _progressManager.MaxHealthLevel >= 5;
        if (maxHealthButton != null)
        {
            maxHealthButton.interactable = !isMaxed && 
                                          _progressManager.CanAfford(_progressManager.GetMaxHealthCost());
        }
        
        if (maxHealthMaxedIndicator != null)
            maxHealthMaxedIndicator.SetActive(isMaxed);
    }

    private void UpdateFireRateDisplay()
    {
        if (fireRateLevelText != null)
            fireRateLevelText.text = "Level: " + _progressManager.FireRateLevel + " / 5";
        
        if (fireRateCostText != null)
            fireRateCostText.text = "Cost: " + _progressManager.GetFireRateCost();
        
        // Check if maxed out
        bool isMaxed = _progressManager.FireRateLevel >= 5;
        if (fireRateButton != null)
        {
            fireRateButton.interactable = !isMaxed && 
                                          _progressManager.CanAfford(_progressManager.GetFireRateCost());
        }
        
        if (fireRateMaxedIndicator != null)
            fireRateMaxedIndicator.SetActive(isMaxed);
    }

    private void UpdateBaseDamageDisplay()
    {
        if (baseDamageLevelText != null)
            baseDamageLevelText.text = "Level: " + _progressManager.BaseDamageLevel + " / 20";
        
        if (baseDamageCostText != null)
            baseDamageCostText.text = "Cost: " + _progressManager.GetBaseDamageCost();
        
        // Check if maxed out
        bool isMaxed = _progressManager.BaseDamageLevel >= 20;
        if (baseDamageButton != null)
        {
            baseDamageButton.interactable = !isMaxed && 
                                            _progressManager.CanAfford(_progressManager.GetBaseDamageCost());
        }
        
        if (baseDamageMaxedIndicator != null)
            baseDamageMaxedIndicator.SetActive(isMaxed);
    }

    /// <summary>Called when Max Health upgrade is purchased.</summary>
    public void OnMaxHealthPurchased()
    {
        if (_progressManager == null)
            _progressManager = ProgressManager.Instance;
        
        if (_progressManager == null)
        {
            Debug.LogError("ProgressManager not found!");
            return;
        }
        
        int cost = _progressManager.GetMaxHealthCost();
        
        if (_progressManager.CanAfford(cost) && _progressManager.MaxHealthLevel < 5)
        {
            _progressManager.Money -= cost;
            _progressManager.MaxHealthLevel += 1;
            
            // Apply upgrade to player if in scene
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.maxHealth = _progressManager.GetActualMaxHealth();
                player.health = player.maxHealth;
            }
            
            RefreshShop();
            
            // Update HUD
            HUD hud = FindObjectOfType<HUD>();
            if (hud != null)
                hud.UpdateMoney();
        }
    }

    /// <summary>Called when Fire Rate upgrade is purchased.</summary>
    public void OnFireRatePurchased()
    {
        if (_progressManager == null)
            _progressManager = ProgressManager.Instance;
        
        if (_progressManager == null)
        {
            Debug.LogError("ProgressManager not found!");
            return;
        }
        
        int cost = _progressManager.GetFireRateCost();
        
        if (_progressManager.CanAfford(cost) && _progressManager.FireRateLevel < 5)
        {
            _progressManager.Money -= cost;
            _progressManager.FireRateLevel += 1;
            RefreshShop();
            
            // Update HUD
            HUD hud = FindObjectOfType<HUD>();
            if (hud != null)
                hud.UpdateMoney();
        }
    }

    /// <summary>Called when Base Damage upgrade is purchased.</summary>
    public void OnBaseDamagePurchased()
    {
        if (_progressManager == null)
            _progressManager = ProgressManager.Instance;
        
        if (_progressManager == null)
        {
            Debug.LogError("ProgressManager not found!");
            return;
        }
        
        int cost = _progressManager.GetBaseDamageCost();
        
        if (_progressManager.CanAfford(cost) && _progressManager.BaseDamageLevel < 20)
        {
            _progressManager.Money -= cost;
            _progressManager.BaseDamageLevel += 1;
            RefreshShop();
            
            // Update HUD
            HUD hud = FindObjectOfType<HUD>();
            if (hud != null)
                hud.UpdateMoney();
        }
    }
}
