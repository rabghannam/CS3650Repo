using UnityEngine;

public class Enemy : GameObjectBase
{
    [Header("Health")]
    public int health = 3;

    [Header("Hit feedback — US-005")]
    public Color hitFlashColor = Color.white;
    public float hitFlashDuration = 0.15f;

    [Header("Lifetime")]
    public float lifetime = 5f;

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
        
        // Award money based on enemy's starting health (which represents "sides")
        // From GDD: "Asteroid destruction | 10 × sides | ×1"
        int moneyReward = 10 * _startingHealth;
        
        ProgressManager progressManager = ProgressManager.Instance;
        if (progressManager != null)
        {
            progressManager.AddMoney(moneyReward);
        }
        
        Destroy(gameObject);
    }
}