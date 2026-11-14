using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Powerup item that can be collected by the player.
/// Provides various power-up effects when collected.
/// Drops from enemies with random probability.
/// </summary>
public class Powerup : GameObjectBase
{
    /// <summary>
    /// Types of powerups available
    /// </summary>
    public enum PowerupType
    {
        RapidFire,
        HealthBoost,
        DamageAmplifier,
        Magnet,
        Shield
    }

    [Header("Powerup Configuration")]
    [Tooltip("Type of powerup this instance represents")]
    public PowerupType powerupType = PowerupType.RapidFire;
    
    [Header("Visual")]
    [Tooltip("Color associated with this powerup type")]
    public Color powerupColor = Color.cyan;
    
    [Header("Movement")]
    public float moveSpeed = 50f; // Not used - powerups stay in place
    
    [Header("Lifetime")]
    public float lifetime = 10f;
    
    [Header("Effect Duration (for temporary effects)")]
    public float duration = 5f;
    
    [Header("Effect Values")]
    [Tooltip("For RapidFire: fire rate multiplier")]
    public float fireRateMultiplier = 2f;
    
    [Tooltip("For HealthBoost: amount of health to restore")]
    public int healthBoostAmount = 25;
    
    [Tooltip("For DamageAmplifier: damage multiplier")]
    public float damageMultiplier = 2f;
    
    [Tooltip("For Magnet: range to attract items")]
    public float magnetRange = 200f;
    
    [Tooltip("For Magnet: speed at which items are attracted")]
    public float magnetSpeed = 300f;

    private float timer;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Set color based on powerup type
            SetColorByType();
        }
    }

    public void SetColorByType()
    {
        if (spriteRenderer == null) return;
        
        switch (powerupType)
        {
            case PowerupType.RapidFire:
                spriteRenderer.color = new Color(1f, 0.5f, 0f); // Orange
                break;
            case PowerupType.HealthBoost:
                spriteRenderer.color = Color.green;
                break;
            case PowerupType.DamageAmplifier:
                spriteRenderer.color = Color.red;
                break;
            case PowerupType.Magnet:
                spriteRenderer.color = new Color(0.5f, 0f, 1f); // Purple
                break;
            case PowerupType.Shield:
                spriteRenderer.color = Color.blue;
                break;
            default:
                spriteRenderer.color = Color.cyan;
                break;
        }
    }

    public override void CustomUpdate(float deltaTime)
    {
        // Powerup stays in place (no falling)
        // position += Vector2.down * moveSpeed * deltaTime;
        
        // Count down lifetime
        timer += deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // When player collects the powerup
        Debug.Log("Powerup collision detected with: " + other.name);
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            Debug.Log("Player detected! Applying powerup: " + powerupType);
            ApplyPowerup(player);
            Destroy(gameObject);
        }
    }
    
    private void ApplyPowerup(Player player)
    {
        switch (powerupType)
        {
            case PowerupType.RapidFire:
                // Double the fire rate
                player.ActivateRapidFire(fireRateMultiplier, duration);
                break;
            
            case PowerupType.HealthBoost:
                // Instantly restore health
                player.ApplyHealthBoost(healthBoostAmount);
                break;
            
            case PowerupType.DamageAmplifier:
                // Double the damage
                player.ActivateDamageAmplifier(damageMultiplier, duration);
                break;
            
            case PowerupType.Magnet:
                // Activate magnet effect
                player.ActivateMagnet(magnetRange, magnetSpeed, duration);
                break;
            
            case PowerupType.Shield:
                // Activate shield (invincibility)
                player.ActivateShield(duration);
                break;
        }
        
        // Visual/feedback effect
        PlayCollectEffect();
    }
    
    private void PlayCollectEffect()
    {
        // Could add particle effect or sound here
        Debug.Log("Powerup collected: " + powerupType.ToString());
    }
    
    /// <summary>
    /// Static method to create a random powerup at a position
    /// </summary>
    public static Powerup CreateRandomPowerup(Vector2 position)
    {
        // Get a random powerup type
        PowerupType randomType = GetRandomPowerupType();
        
        // In a real Unity project, you would instantiate a prefab
        // For now, we'll just return the type that should be used
        Debug.Log("Creating random powerup: " + randomType.ToString());
        
        // This would be: Instantiate(prefab, position, Quaternion.identity)
        // with the appropriate type set
        
        return null; // Placeholder - actual instantiation happens in Enemy.cs
    }
    
    /// <summary>
    /// Gets a random powerup type with equal probability
    /// </summary>
    public static PowerupType GetRandomPowerupType()
    {
        PowerupType[] allTypes = (PowerupType[])System.Enum.GetValues(typeof(PowerupType));
        return allTypes[Random.Range(0, allTypes.Length)];
    }
}
