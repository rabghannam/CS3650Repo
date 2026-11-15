using UnityEngine;

public class Enemy : GameObjectBase
{
    [Header("Health")]
    public int health = 3;

    [Header("Hit feedback � US-005")]
    public Color hitFlashColor = Color.white;
    public float hitFlashDuration = 0.15f;

    [Header("Lifetime")]
    public float lifetime = 5f;
    [Header("Item Drops")]
    [Tooltip("65% chance to drop money")]
    public float moneyDropRate = 0.65f;
    [Tooltip("30% chance to drop powerup (RapidFire, HealthBoost, DamageAmplifier, Magnet, Shield)")]
    public float powerupDropRate = 0.30f;
    [Tooltip("5% chance to drop nothing")]
    public float noDropRate = 0.05f;
    
    [Tooltip("Prefab to instantiate when dropping money")]
    public GameObject moneyPrefab;
    [Tooltip("Prefab to instantiate when dropping powerup")]
    public GameObject powerupPrefab;

    [HideInInspector] public bool isAlive = true;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float hitFlashTimer;
    private float lifetimeTimer;
    private int _startingHealth;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
        lifetimeTimer = lifetime;
        _startingHealth = health; // Store original health for money calculation
    }

    public override void CustomUpdate(float deltaTime)
    {
        if (hitFlashTimer > 0f)
        {
            hitFlashTimer -= deltaTime;                          // frame-rate independent
            if (hitFlashTimer <= 0f && spriteRenderer != null)
                spriteRenderer.color = originalColor;            // effect clearly visible then removed
        }

        // Count down lifetime
        lifetimeTimer -= deltaTime;
        if (lifetimeTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>US-005: damage applied instantly, health reduces visibly, visible hit effect.</summary>
    public void takeDamage(int amount)
    {
        if (!isAlive) return;
        health -= amount;
        playHitEffect();
        if (health <= 0) Die();
    }

    public void playHitEffect()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = hitFlashColor;                // bright, distinguishable flash
        hitFlashTimer = hitFlashDuration;
    }

    private void Die()
    {
        isAlive = false;
        
        // Drop exactly one money item with value based on asteroid sides (health)
        // Money value scales linearly with asteroid sides: 10 * sides
        if (moneyPrefab != null)
        {
            GameObject moneyObj = Instantiate(moneyPrefab, transform.position, Quaternion.identity);
            Money money = moneyObj.GetComponent<Money>();
            if (money != null)
            {
                // Set money amount based on enemy's starting health (which represents "sides")
                // From GDD: "Asteroid destruction | 10 × sides"
                money.amount = 10 * _startingHealth;
            }
        }
        
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Drops a random powerup at the enemy's current position.
    /// </summary>
    private void DropPowerup()
    {
        if (powerupPrefab != null)
        {
            GameObject powerupObj = Instantiate(powerupPrefab, transform.position, Quaternion.identity);
            Powerup powerup = powerupObj.GetComponent<Powerup>();
            if (powerup != null)
            {
                // Set a random powerup type
                powerup.powerupType = Powerup.GetRandomPowerupType();
                // Update color based on type
                powerup.SetColorByType();
                Debug.Log("Dropped powerup: " + powerup.powerupType.ToString());
            }
        }
    }
    
    /// <summary>
    /// Drops an item at the enemy's current position.
    /// </summary>
    /// <param name="prefab">The prefab to instantiate</param>
    private void DropItem(GameObject prefab)
    {
        if (prefab != null)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
        }
    }
}