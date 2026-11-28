using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [Header("References")]
    public Player player;
    public InputHandler inputHandler;
    public CollisionSystem collisionSystem;
    public HUD hud;
    public GameOverScreen gameOverScreen;
    public PauseMenu pauseMenu;

    [Header("Entities")]
    public List<Enemy> enemies = new List<Enemy>();
    public List<Bullet> bullets = new List<Bullet>();
    public List<Powerup> powerups = new List<Powerup>();

    public bool isGameOver { get; private set; }

    private void Start()
    {
        player.game = this;
        player.input = inputHandler;
        player.hud = hud;

        // Temporal Echo System (GDD 4.3): one recorder per run, wired to the player.
        EchoRecorder echoRecorder = GetComponent<EchoRecorder>();
        if (echoRecorder == null) echoRecorder = gameObject.AddComponent<EchoRecorder>();
        echoRecorder.player = player;
        echoRecorder.game = this;
        echoRecorder.bulletPrefab = player.bulletPrefab;

        
        // Load upgrades from ProgressManager
        ProgressManager progressManager = ProgressManager.Instance;
        if (progressManager != null)
        {
            player.maxHealth = progressManager.GetActualMaxHealth();
            player.health = player.maxHealth;
            // Note: Fire rate and damage upgrades are stored and applied where needed
        }
        
        hud.updateHealth(player.health, player.maxHealth);       // initial HUD state (US-011)
    }

    private void Update()
    {
        if (isGameOver) return;
        if (pauseMenu != null && pauseMenu.IsPaused) return;

        // 1. Entities update themselves (movement, timers) — frame-rate independent.
        // 2. Per-frame collision checks (sequence diagram: Game -> CollisionSystem).
        collisionSystem.resolveCollisions(bullets, enemies, player);
    }

    /// <summary>US-008: death condition -> game over transition.</summary>
    public void notifyDeath(Player p)
    {
        isGameOver = true;
        
        // Reset player progress on death
        ProgressManager progressManager = ProgressManager.Instance;
        if (progressManager != null)
        {
            progressManager.ResetProgress();
        }
        
        triggerGameOver();
    }

    public void triggerGameOver()
    {
        // The player stays in Dying (US-008); GameOver is a post-death
        // transition, and isDead() already covers Dying for all gameplay gates.
        if (gameOverScreen != null) gameOverScreen.show();      // transitions to game over screen
    }

    private void LateUpdate()
    {
        // Remove destroyed entities from lists
        enemies.RemoveAll(e => e == null);
        bullets.RemoveAll(b => b == null);
        powerups.RemoveAll(p => p == null);
    }
}