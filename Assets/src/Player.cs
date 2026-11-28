using UnityEngine;

public class Player : GameObjectBase
{
    [Header("Movement � US-001")]
    public const float BASE_SPEED = 300f;        // units (pixels) per second
    public float moveSpeed = BASE_SPEED;

    [Header("Health � US-006 / US-011")]
    public const int BASE_HEALTH = 50;
    public int health = BASE_HEALTH;
    public int maxHealth = BASE_HEALTH;

    [Header("I-frames � US-009 (matches red flash)")]
    public const float IFRAME_DURATION = 1f;
    public float iFrameTimer;

    [Header("Death � US-008")]
    public GameObject deathEffectPrefab;
    public float deathEffectDuration = 1.5f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public const float FIRE_RATE = 5f; // shots per second
    public enum FireMode { Manual, Automatic }
    public FireMode fireMode = FireMode.Manual;
    private float fireTimer;
    
    [Header("Powerups")]
    private float rapidFireMultiplier = 1f;
    private float damageMultiplier = 1f;
    private bool hasMagnet = false;
    private float magnetRange = 200f;
    private float magnetSpeed = 300f;
    private bool hasShield = false;
    
    // Timers for temporary powerups
    private float rapidFireTimer = 0f;
    private float damageAmplifierTimer = 0f;
    private float magnetTimer = 0f;
    private float shieldTimer = 0f;

    // Absorbs float residue when a duration is ticked down in many small
    // steps (e.g. 30 x 0.01s from 0.3s leaves ~1.5e-8 above zero).
    private const float TIMER_EXPIRY_EPSILON = 0.0001f;

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
        // Don't process input if game is paused or over
        if (game != null && game.pauseMenu != null && game.pauseMenu.IsPaused) return;
        if (isDead) return;

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
                // Player cannot move or fire after death � no input accepted.
                break;
        }
    }

    /// <summary>US-001: WASD + arrows, 300 u/s, frame-rate independent, instant direction changes.</summary>
    private void handleInput()
    {
        if (input == null) return;
        var s = input.readKeyboard();
        Vector2 dir = s.moveDirection;                            // instantaneous direction change
        velocity = dir.normalized * moveSpeed;                   // 300 units/second, normalized diagonals
        position += velocity * Time.deltaTime;                   // frame-rate independent

        // Toggle fire mode with Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            fireMode = fireMode == FireMode.Manual ? FireMode.Automatic : FireMode.Manual;
            fireTimer = 0f; // Reset timer to fire immediately on mode change
        }

        // Update powerup timers
        UpdatePowerupTimers(Time.deltaTime);

        // Shooting logic
        bool spacePressed = Input.GetKey(KeyCode.Space);
        bool mousePressed = Input.GetMouseButton(0);
        
        // Get upgraded fire rate from ProgressManager
        float actualFireRate = FIRE_RATE;
        ProgressManager progressManager = ProgressManager.Instance;
        if (progressManager != null)
        {
            actualFireRate = progressManager.GetActualFireRate();
        }
        float fireInterval = 1f / actualFireRate;

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
                fireTimer -= Time.deltaTime;
                if (fireTimer <= 0f)
                {
                    SpawnBullet();
                    fireTimer = fireInterval;
                }
            }
        }
    }
    
    /// <summary>
    /// Updates all active powerup timers
    /// </summary>
    private void UpdatePowerupTimers(float deltaTime)
    {
        // Update rapid fire timer
        if (rapidFireTimer > 0f)
        {
            rapidFireTimer -= deltaTime;
            if (rapidFireTimer <= TIMER_EXPIRY_EPSILON)
            {
                rapidFireMultiplier = 1f;
                rapidFireTimer = 0f;
                Debug.Log("Rapid Fire expired");
            }
        }
        
        // Update damage amplifier timer
        if (damageAmplifierTimer > 0f)
        {
            damageAmplifierTimer -= deltaTime;
            if (damageAmplifierTimer <= TIMER_EXPIRY_EPSILON)
            {
                damageMultiplier = 1f;
                damageAmplifierTimer = 0f;
                Debug.Log("Damage Amplifier expired");
            }
        }
        
        // Update magnet timer
        if (magnetTimer > 0f)
        {
            magnetTimer -= deltaTime;
            if (magnetTimer <= TIMER_EXPIRY_EPSILON)
            {
                hasMagnet = false;
                magnetTimer = 0f;
                Debug.Log("Magnet expired");
            }
        }
        
        // Update shield timer
        if (shieldTimer > 0f)
        {
            shieldTimer -= deltaTime;
            if (shieldTimer <= TIMER_EXPIRY_EPSILON)
            {
                hasShield = false;
                shieldTimer = 0f;
                // Don't override state if player is in another state
                if (state == PlayerState.Invulnerable && iFrameTimer <= 0f)
                {
                    state = PlayerState.Alive;
                }
                Debug.Log("Shield expired");
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
                // Apply base damage upgrade from ProgressManager
                ProgressManager progressManager = ProgressManager.Instance;
                if (progressManager != null)
                {
                    newBullet.damage = progressManager.GetActualBaseDamage();
                }
                game.bullets.Add(newBullet);
            }
        }
    }
    
    #region Powerup Methods
    
    /// <summary>
    /// Activates rapid fire powerup
    /// </summary>
    public void ActivateRapidFire(float multiplier, float duration)
    {
        rapidFireMultiplier = multiplier;
        rapidFireTimer = duration;
        Debug.Log("Rapid Fire activated! Fire rate multiplier: " + multiplier);
    }
    
    /// <summary>
    /// Applies instant health boost
    /// </summary>
    public void ApplyHealthBoost(int amount)
    {
        health = Mathf.Min(maxHealth, health + amount);
        hud.updateHealth(health, maxHealth);
        Debug.Log("Health boost! +" + amount + " health. Current: " + health);
    }
    
    /// <summary>
    /// Activates damage amplifier powerup
    /// </summary>
    public void ActivateDamageAmplifier(float multiplier, float duration)
    {
        damageMultiplier = multiplier;
        damageAmplifierTimer = duration;
        Debug.Log("Damage Amplifier activated! Damage multiplier: " + multiplier);
    }
    
    /// <summary>
    /// Activates magnet powerup
    /// </summary>
    public void ActivateMagnet(float range, float speed, float duration)
    {
        hasMagnet = true;
        magnetRange = range;
        magnetSpeed = speed;
        magnetTimer = duration;
        Debug.Log("Magnet activated! Range: " + range + ", Speed: " + speed);
    }
    
    /// <summary>
    /// Activates shield powerup (invincibility)
    /// </summary>
    public void ActivateShield(float duration)
    {
        hasShield = true;
        shieldTimer = duration;
        state = PlayerState.Invulnerable;
        iFrameTimer = duration;
        Debug.Log("Shield activated! Duration: " + duration);
    }
    
    /// <summary>
    /// Returns the current fire rate multiplier (affected by RapidFire powerup)
    /// </summary>
    public float GetFireRateMultiplier()
    {
        return rapidFireMultiplier;
    }
    
    /// <summary>
    /// Returns whether magnet is active
    /// </summary>
    public bool HasMagnet()
    {
        return hasMagnet;
    }
    
    /// <summary>
    /// Gets magnet parameters
    /// </summary>
    public void GetMagnetParams(out float range, out float speed)
    {
        range = magnetRange;
        speed = magnetSpeed;
    }
    
    #endregion

    /// <summary>
    /// US-006: exactly 1 damage per enemy bullet hit, applied instantly,
    /// clamped so health never drops below 0. US-008: death at 0.
    /// Shield powerup blocks all damage.
    /// </summary>
    public void takeDamage(int amount)
    {
        // Shield powerup blocks all damage
        if (hasShield)
        {
            Debug.Log("Shield blocked damage!");
            return;
        }
        
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
            else if (invincibilityEnabled())
            {
                state = PlayerState.Invulnerable;                // Alive -> Invulnerable (US-009)
                iFrameTimer = IFRAME_DURATION;
                hud.triggerRedFlash();                            // 1 s red flash
            }
        }
        // While Invulnerable: extra hits do nothing (i-frames).
        // While Dying/GameOver: damage is ignored.
    }

    /// <summary>US-009: i-frames can be toggled OFF in settings; default ON when no settings exist.</summary>
    private bool invincibilityEnabled()
    {
        SettingsManager settings = SettingsManager.Instance;
        return settings == null || settings.InvincibilityEnabled;
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