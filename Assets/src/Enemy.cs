using UnityEngine;

public class Enemy : GameObjectBase
{
    [Header("Health")]
    public int health = 3;

    [Header("Hit feedback � US-005")]
    public Color hitFlashColor = Color.white;
    public float hitFlashDuration = 0.15f;

    [Header("Health Color Indicator")]
    public bool useHealthColorGradient = true;
    public Color fullHealthColor = Color.green;
    public Color threeQuartersHealthColor = new Color(0.75f, 1f, 0.5f);
    public Color halfHealthColor = Color.yellow;
    public Color quarterHealthColor = new Color(1f, 0.65f, 0f);
    public Color lowHealthColor = Color.red;

    [Header("Lifetime")]
    public float lifetime = 5f;
    [Header("Item Drops")]
    [Tooltip("75% chance to drop money")]
    public float moneyDropRate = 0.75f;
    [Tooltip("25% chance to drop powerup")]
    public float powerupDropRate = 0.25f;
    
    [Tooltip("Prefab to instantiate when dropping money")]
    public GameObject moneyPrefab;
    [Tooltip("Prefab to instantiate when dropping powerup")]
    public GameObject powerupPrefab;

    [HideInInspector] public bool isAlive = true;

    public SpriteRenderer spriteRenderer;
    public Color originalColor;
    public float hitFlashTimer;
    public float lifetimeTimer;
    private int _startingHealth;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
        lifetimeTimer = lifetime;
        _startingHealth = health; // Store original health for money calculation
        UpdateHealthColor();
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
        UpdateHealthColor();
        if (health <= 0) Die();
    }

    public void playHitEffect()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = hitFlashColor;                // bright, distinguishable flash
        hitFlashTimer = hitFlashDuration;
    }

    /// <summary>
    /// Call this from derived classes to set custom max health.
    /// Must be called in Start() or later, not in Awake().
    /// </summary>
    protected void SetMaxHealth(int maxHealth)
    {
        health = maxHealth;
        _startingHealth = maxHealth;
        UpdateHealthColor();
    }

    /// <summary>
    /// Updates enemy color based on health percentage.
    /// Green at 100%, Green-Yellow at 75%, Yellow at 50%, Yellow-Red at 25%, Red at <25%
    /// Smooth gradient transitions between color states.
    /// </summary>
    private void UpdateHealthColor()
    {
        if (spriteRenderer == null || !useHealthColorGradient) return;
        
        float maxHealth = Mathf.Max(1, _startingHealth);
        float healthPercentage = (float)health / maxHealth;
        Color newColor;
        
        if (healthPercentage >= 0.75f)
        {
            float t = Mathf.InverseLerp(0.75f, 1.0f, healthPercentage);
            newColor = Color.Lerp(threeQuartersHealthColor, fullHealthColor, t);
        }
        else if (healthPercentage >= 0.5f)
        {
            float t = Mathf.InverseLerp(0.5f, 0.75f, healthPercentage);
            newColor = Color.Lerp(halfHealthColor, threeQuartersHealthColor, t);
        }
        else if (healthPercentage >= 0.25f)
        {
            float t = Mathf.InverseLerp(0.25f, 0.5f, healthPercentage);
            newColor = Color.Lerp(quarterHealthColor, halfHealthColor, t);
        }
        else
        {
            float t = Mathf.InverseLerp(0f, 0.25f, healthPercentage);
            newColor = Color.Lerp(lowHealthColor, quarterHealthColor, t);
        }
        
        if (hitFlashTimer <= 0f)
        {
            spriteRenderer.color = newColor;
        }
        originalColor = newColor;
    }

    private void Die()
    {
        isAlive = false;
        
        // Determine what to drop based on drop rates
        float dropRoll = Random.value;
        
        if (dropRoll < moneyDropRate)
        {
            // Drop money (75% chance)
            if (moneyPrefab != null)
            {
                GameObject moneyObj = Instantiate(moneyPrefab, transform.position, Quaternion.identity);
                Money money = moneyObj.GetComponent<Money>();
                if (money != null)
                {
                    money.amount = 10 * _startingHealth;
                }
            }
        }
        else if (dropRoll < moneyDropRate + powerupDropRate)
        {
            // Drop powerup (25% chance)
            DropPowerup();
        }
        // else: no drop (implicit, 0% since 75% + 25% = 100%)

        // Award base money for defeating enemy
        ProgressManager progressManager = ProgressManager.Instance;
        if (progressManager != null)
        {
            progressManager.AddMoney(10 * _startingHealth);
        }

        // Destroy is not allowed outside Play mode (edit-mode unit tests);
        // DestroyImmediate is only used there so tests can exercise death logic.
        if (Application.isPlaying)
        {
            Destroy(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
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
    
}