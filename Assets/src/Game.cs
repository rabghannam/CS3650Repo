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

    public bool isGameOver { get; private set; }

    private void Start()
    {
        player.game = this;
        player.input = inputHandler;
        player.hud = hud;
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
        triggerGameOver();
    }

    public void triggerGameOver()
    {
        player.state = Player.PlayerState.GameOver;             // Dying -> GameOver
        gameOverScreen.show();                                  // transitions to game over screen
    }

    private void LateUpdate()
    {
        // Remove destroyed entities from lists
        enemies.RemoveAll(e => e == null);
        bullets.RemoveAll(b => b == null);
    }
}