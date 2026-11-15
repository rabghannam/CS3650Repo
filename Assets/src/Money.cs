using UnityEngine;

/// <summary>
/// Money item that can be collected by the player.
/// Drops from destroyed asteroids with value based on sides.
/// </summary>
public class Money : GameObjectBase
{
    [Header("Value")]
    public int amount = 10;
    
    [Header("Movement")]
    public float moveSpeed = 50f;
    
    [Header("Lifetime")]
    public float lifetime = 10f;
    
    [Header("Visual Effects")]
    public GameObject collectEffectPrefab;
    public float collectEffectDuration = 0.5f;
    
    private float timer;
    private bool isCollected = false;
    
    public override void CustomUpdate(float deltaTime)
    {
        // Money stays in place (no falling) - can be collected by player
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
        if (isCollected) return;
        
        // When player collects money
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            Collect();
        }
    }
    
    /// <summary>
    /// Collects the money, adds to player's total, and plays visual effect.
    /// </summary>
    private void Collect()
    {
        isCollected = true;
        
        // Add money to ProgressManager
        ProgressManager progressManager = ProgressManager.Instance;
        if (progressManager != null)
        {
            progressManager.AddMoney(amount);
        }
        
        // Play visual confirmation effect
        PlayCollectEffect();
        
        // Update HUD
        HUD hud = FindObjectOfType<HUD>();
        if (hud != null)
        {
            hud.UpdateMoney();
        }
        
        // Debug log
        Debug.Log("Collected " + amount + " money!");
        
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Plays a visual effect when money is collected.
    /// </summary>
    private void PlayCollectEffect()
    {
        if (collectEffectPrefab != null)
        {
            GameObject effect = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, collectEffectDuration);
        }
        else
        {
            // Default effect: scale up and fade out
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                // Simple visual feedback
                transform.localScale *= 1.5f;
            }
        }
    }
}
