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
    [Tooltip("80% chance to drop money")]
    public float moneyDropRate = 0.80f;
    [Tooltip("15% chance to drop temporary upgrade")]
    public float upgradeDropRate = 0.15f;
    [Tooltip("5% chance to drop nothing")]
    public float noDropRate = 0.05f;
    
    [Tooltip("Prefab to instantiate when dropping money")]
    public GameObject moneyPrefab;
    [Tooltip("Prefab to instantiate when dropping temporary upgrade")]
    public GameObject temporaryUpgradePrefab;

    [HideInInspector] public bool isAlive = true;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float hitFlashTimer;
    private float lifetimeTimer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
        lifetimeTimer = lifetime;
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
        
        // US: Enemy item drops - each enemy drops exactly one item based on defined percentages
        // Total drop rate: 100% (80% money + 15% upgrade + 5% no drop)
        float dropRoll = Random.value;
        float cumulativeRate = 0f;
        
        // Check money drop (80%)
        cumulativeRate += moneyDropRate;
        if (dropRoll < cumulativeRate)
        {
            DropItem(moneyPrefab);
        }
        // Check temporary upgrade drop (15%)
        else if (dropRoll < cumulativeRate + upgradeDropRate)
        {
            DropItem(temporaryUpgradePrefab);
        }
        // else: no drop (5%)
        
        Destroy(gameObject);
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