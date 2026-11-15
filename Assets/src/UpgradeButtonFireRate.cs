using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Button script for purchasing Fire Rate upgrades.
/// Displays current level, cost for next level, and handles purchase logic.
/// </summary>
public class UpgradeButtonFireRate : MonoBehaviour
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

        int currentLevel = _progressManager.FireRateLevel;
        int maxLevel = 5;
        int cost = _progressManager.GetFireRateCost();

        // Update level display
        if (levelText != null)
            levelText.text = "Fire Rate: " + currentLevel + " / " + maxLevel;

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

        int cost = _progressManager.GetFireRateCost();
        
        // Check if can purchase
        if (_progressManager.FireRateLevel < 5 && _progressManager.CanAfford(cost))
        {
            _progressManager.Money -= cost;
            _progressManager.FireRateLevel += 1;
            
            // Apply the upgrade to player (if player exists in scene)
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                // Update player's fire rate
                // Note: This would need to be applied to the player's shooting logic
                // For now, we just store it in ProgressManager
            }
            
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
            Debug.Log("Cannot purchase Fire Rate: Level=" + _progressManager.FireRateLevel + " Money=" + _progressManager.Money + " Cost=" + cost);
        }
    }
}
