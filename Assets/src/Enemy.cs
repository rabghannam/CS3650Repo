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
        
        // Award money based on enemy's starting health (which represents "sides")
        // From GDD: "Asteroid destruction | 10 � sides | �1"
        int moneyReward = 10 * _startingHealth;
        
        ProgressManager progressManager = ProgressManager.Instance;
        if (progressManager != null)
        {
            progressManager.AddMoney(moneyReward);
        }
        
        Destroy(gameObject);
    }
}