using UnityEngine;

public class Enemy : GameObjectBase
{
    [Header("Health")]
    public int health = 3;

    [Header("Hit feedback — US-005")]
    public Color hitFlashColor = Color.white;
    public float hitFlashDuration = 0.15f;

    [HideInInspector] public bool isAlive = true;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float hitFlashTimer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    public override void CustomUpdate(float deltaTime)
    {
        if (hitFlashTimer > 0f)
        {
            hitFlashTimer -= deltaTime;                          // frame-rate independent
            if (hitFlashTimer <= 0f && spriteRenderer != null)
                spriteRenderer.color = originalColor;            // effect clearly visible then removed
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
        Destroy(gameObject);
    }
}