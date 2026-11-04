using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Button script for purchasing Base Damage upgrades.
/// Displays current level, cost for next level, and handles purchase logic.
/// </summary>
public class UpgradeButtonBaseDamage : MonoBehaviour
{
    [Header("UI References")]
    public Text levelText;
    public Text costText;
    public Image icon;
    public Button purchaseButton;
    public GameObject maxedOutIndicator;
    public Text buttonText;

    private ProgressManager _progressManager;

    private void Start()
    {
        _progressManager = ProgressManager.Instance;
        RefreshButton();
        
        // Auto-connect button click event
        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchase);
        }
    }

    private void OnEnable()
    {
        if (_progressManager == null)
            _progressManager = ProgressManager.Instance;
        RefreshButton();
    }
    
    private void OnDisable()
    {
        // Remove listener to prevent duplicates
        if (purchaseButton != null)
        {
            purchaseButton.onClick.RemoveListener(OnPurchase);
        }
    }

    /// <summary>Refresh the button display with current upgrade data.</summary>
    public void RefreshButton()
    {
        if (_progressManager == null)
            _progressManager = ProgressManager.Instance;

        int currentLevel = _progressManager.BaseDamageLevel;
        int maxLevel = 20;
        int cost = _progressManager.GetBaseDamageCost();

        // Update level display
        if (levelText != null)
            levelText.text = "Base Damage: " + currentLevel + " / " + maxLevel;

        // Update cost display
        if (costText != null)
            costText.text = "Cost: " + cost;

        // Check if maxed out
        bool isMaxed = currentLevel >= maxLevel;
        bool canAfford = _progressManager.CanAfford(cost);

        // Update button state
        if (purchaseButton != null)
        {
            purchaseButton.interactable = !isMaxed && canAfford;
            
            if (isMaxed && buttonText != null)
                buttonText.text = "MAXED";
            else if (!canAfford && buttonText != null)
                buttonText.text = "NEED " + cost + " MONEY";
            else if (buttonText != null)
                buttonText.text = "BUY LEVEL " + (currentLevel + 1);
        }

        // Show maxed out indicator
        if (maxedOutIndicator != null)
            maxedOutIndicator.SetActive(isMaxed);
    }

    /// <summary>Called when the purchase button is clicked.</summary>
    public void OnPurchase()
    {
        // Ensure ProgressManager exists
        if (_progressManager == null)
            _progressManager = ProgressManager.Instance;
        
        if (_progressManager == null)
        {
            Debug.LogError("ProgressManager not found! Create a ProgressManager GameObject in the scene.");
            return;
        }

        int cost = _progressManager.GetBaseDamageCost();
        
        // Check if can purchase
        if (_progressManager.BaseDamageLevel < 20 && _progressManager.CanAfford(cost))
        {
            _progressManager.Money -= cost;
            _progressManager.BaseDamageLevel += 1;
            
            // Apply the upgrade to bullet prefab (if needed)
            // Note: This would need to be applied to the bullet prefab or player's bullet spawning logic
            // For now, we just store it in ProgressManager
            
            RefreshButton();
            
            // Refresh the main shop if it exists
            UpgradeShop shop = FindObjectOfType<UpgradeShop>();
            if (shop != null)
                shop.RefreshShop();
            
            // Force HUD update
            HUD hud = FindObjectOfType<HUD>();
            if (hud != null)
                hud.UpdateMoney();
        }
        else
        {
            Debug.Log("Cannot purchase Base Damage: Level=" + _progressManager.BaseDamageLevel + " Money=" + _progressManager.Money + " Cost=" + cost);
        }
    }
}
