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

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public const float FIRE_RATE = 5f; // shots per second
    public enum FireMode { Manual, Automatic }
    public FireMode fireMode = FireMode.Manual;
    private float fireTimer;

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

        // Toggle fire mode with Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            fireMode = fireMode == FireMode.Manual ? FireMode.Automatic : FireMode.Manual;
            fireTimer = 0f; // Reset timer to fire immediately on mode change
        }

        // Shooting logic
        bool spacePressed = Input.GetKey(KeyCode.Space);
        bool mousePressed = Input.GetMouseButton(0);
        float fireInterval = 1f / FIRE_RATE;

        if (bulletPrefab != null)
        {
            if (fireMode == FireMode.Manual)
            {
                // Manual: fire on key/mouse down
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    SpawnBullet();
                }
            }
            else // Automatic
            {
                // Automatic: fire continuously at FIRE_RATE while holding
                if (spacePressed || mousePressed)
                {
                    fireTimer -= Time.deltaTime;
                    if (fireTimer <= 0f)
                    {
                        SpawnBullet();
                        fireTimer = fireInterval;
                    }
                }
            }
        }
    }

    private void SpawnBullet()
    {
        if (bulletPrefab != null && game != null)
        {
            GameObject newBulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Bullet newBullet = newBulletObj.GetComponent<Bullet>();
            if (newBullet != null)
            {
                game.bullets.Add(newBullet);
            }
        }
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