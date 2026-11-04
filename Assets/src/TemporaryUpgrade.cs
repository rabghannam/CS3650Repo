using UnityEngine;

/// <summary>
/// Temporary upgrade item that can be collected by the player.
/// Provides a temporary power-up effect.
/// Drops from enemies with 15% probability.
/// </summary>
public class TemporaryUpgrade : GameObjectBase
{
    //[Header("Upgrade Type")]
    public enum UpgradeType { SpeedBoost, DamageBoost, Invincibility }
    public UpgradeType upgradeType = UpgradeType.SpeedBoost;
    
    //[Header("Duration")]
    public float duration = 5f;
    
    //[Header("Movement")]
    public float moveSpeed = 50f;
    
    //[Header("Lifetime")]
    public float lifetime = 10f;
    
    private float timer;
    
    public override void CustomUpdate(float deltaTime)
    {
        // Temporary upgrade moves downward slowly
        position += Vector2.down * moveSpeed * deltaTime;
        
        // Count down lifetime
        timer += deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // When player collects the upgrade
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            ApplyUpgrade(player);
            Destroy(gameObject);
        }
    }
    
    private void ApplyUpgrade(Player player)
    {
        switch (upgradeType)
        {
            case UpgradeType.SpeedBoost:
                player.moveSpeed *= 1.5f;
                break;
            case UpgradeType.DamageBoost:
                // Damage boost logic would go here
                break;
            case UpgradeType.Invincibility:
                player.state = Player.PlayerState.Invulnerable;
                player.iFrameTimer = duration;
                break;
        }
        
        // Note: You would typically want to track and remove the upgrade after duration
        // This is a simplified implementation
    }
}
