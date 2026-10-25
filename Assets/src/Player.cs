using UnityEngine;

public class Player : GameObjectBase
{
    [Header("Movement — US-001")]
    public const float BASE_SPEED = 300f;        // units (pixels) per second
    public float moveSpeed = BASE_SPEED;

    [Header("Health — US-006 / US-011")]
    public const int BASE_HEALTH = 50;
    public int health = BASE_HEALTH;
    public int maxHealth = BASE_HEALTH;

    [Header("I-frames — US-009 (matches red flash)")]
    public const float IFRAME_DURATION = 1f;
    public float iFrameTimer;

    [Header("Death — US-008")]
    public GameObject deathEffectPrefab;
    public float deathEffectDuration = 1.5f;

    // State machine states
    public enum PlayerState { Alive, Invulnerable, Dying, GameOver }
    public PlayerState state = PlayerState.Alive;
    public bool isDead => state == PlayerState.Dying || state == PlayerState.GameOver;
    public bool isAlive() => !isDead;

    [HideInInspector] public InputHandler input;
    [HideInInspector] public HUD hud;
    [HideInInspector] public Game game;

    private Vector2 velocity;

    public override void CustomUpdate(float deltaTime)
    {
        switch (state)
        {
            case PlayerState.Alive:
                handleInput();
                break;

            case PlayerState.Invulnerable:
                handleInput();                                   // player can still move during i-frames
                iFrameTimer -= deltaTime;                        // frame-rate independent countdown
                if (iFrameTimer <= 0f)
                {
                    iFrameTimer = 0f;
                    state = PlayerState.Alive;                   // Invulnerable -> Alive
                }
                break;

            case PlayerState.Dying:
                // Wait for death animation, then transition (handled by Game)
                break;

            case PlayerState.GameOver:
                // Player cannot move or fire after death — no input accepted.
                break;
        }
    }

    /// <summary>US-001: WASD + arrows, 300 u/s, frame-rate independent, instant direction changes.</summary>
    private void handleInput()
    {
        if (input == null) return;
        var s = input.readKeyboard();
        Vector2 dir = s.moveDirection;                            // instantaneous direction change
        velocity = dir * moveSpeed;                              // 300 units/second
        position += velocity * Time.deltaTime;                   // frame-rate independent
    }

    /// <summary>
    /// US-006: exactly 1 damage per enemy bullet hit, applied instantly,
    /// clamped so health never drops below 0. US-008: death at 0.
    /// </summary>
    public void takeDamage(int amount)
    {
        if (state == PlayerState.Alive)
        {
            health = Mathf.Max(0, health - amount);              // instant, clamped at 0
            hud.updateHealth(health, maxHealth);                 // real-time HUD (US-011)

            if (health == 0)
            {
                state = PlayerState.Dying;                       // Alive -> Dying (US-008)
                playDeathEffect();
                game.notifyDeath(this);
            }
            else
            {
                state = PlayerState.Invulnerable;                // Alive -> Invulnerable (US-009)
                iFrameTimer = IFRAME_DURATION;
                hud.triggerRedFlash();                            // 1 s red flash
            }
        }
        // While Invulnerable: extra hits do nothing (i-frames).
        // While Dying/GameOver: damage is ignored.
    }

    private void playDeathEffect()
    {
        if (deathEffectPrefab != null)
        {
            GameObject fx = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(fx, deathEffectDuration);
        }
    }

    public int getHealth() => health;
}