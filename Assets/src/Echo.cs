using UnityEngine;

/// <summary>
/// Deployed temporal echo (GDD 4.3): replays the recorded path with 100%
/// accuracy, fires in sync with the player (5 shots/sec) with a damage
/// multiplier, auto-collects drops within 32 units, and acts as a decoy
/// that absorbs one hit. Visual: semi-transparent cyan ghost (60% opacity).
/// </summary>
public class Echo : GameObjectBase
{
    public const float COLLECTION_RANGE = 32f;     // GDD: auto-collect drops within 32 units
    public const float ECHO_VISUAL_OPACITY = 0.6f; // GDD: 60% opacity cyan ghost

    [Header("Wiring (set by EchoRecorder.Deploy)")]
    public EchoRecording recording;
    public Player player;
    public Game game;
    public GameObject bulletPrefab;
    public float damageMultiplier = 1f;            // GDD: 1.0x (upgradable to 2.0x)

    [Header("Decoy")]
    public int decoyHits = 1;                      // GDD: absorbs 1 hit

    private float _playTime;
    private float _fireTimer;

    public bool playbackFinished { get; private set; }

    private void Start()
    {
        // Semi-transparent cyan ghost visual, when a sprite is present.
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(0f, 1f, 1f, ECHO_VISUAL_OPACITY);
    }

    /// <summary>True while the decoy charge is available; consumes it otherwise.</summary>
    public bool TryAbsorbHit()
    {
        if (decoyHits <= 0) return false;
        decoyHits--;
        return true;
    }

    public override void CustomUpdate(float deltaTime)
    {
        if (playbackFinished || recording == null) return;

        // Movement: replay the recorded path with 100% accuracy.
        _playTime += deltaTime;
        position = recording.Evaluate(_playTime);
        if (_playTime >= recording.Duration)
        {
            playbackFinished = true;               // rest at the final recorded position
            return;
        }

        // Firing: same 5 shots/sec cadence as the player.
        if (game != null && bulletPrefab != null && player != null && player.isAlive())
        {
            _fireTimer -= deltaTime;
            if (_fireTimer <= 0f)
            {
                Fire();
                _fireTimer = 1f / Player.FIRE_RATE;
            }
        }

        // Collection: auto-collect drops within 32 units.
        CollectDrops();
    }

    private void Fire()
    {
        Bullet newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity)
            .GetComponent<Bullet>();
        if (newBullet == null) return;

        int baseDamage = 1;
        ProgressManager progressManager = ProgressManager.Instance;
        if (progressManager != null)
            baseDamage = progressManager.GetActualBaseDamage();

        newBullet.damage = Mathf.RoundToInt(baseDamage * damageMultiplier);
        newBullet.owner = Bullet.Owner.Player;
        game.bullets.Add(newBullet);
    }

    private void CollectDrops()
    {
        if (game == null) return;

        for (int i = game.powerups.Count - 1; i >= 0; i--)
        {
            Powerup powerup = game.powerups[i];
            if (powerup == null) continue;

            if (Vector2.Distance(powerup.position, position) <= COLLECTION_RANGE)
            {
                powerup.ApplyPowerup(player);
                game.powerups.RemoveAt(i);
                if (Application.isPlaying) Destroy(powerup.gameObject);
            }
        }
    }
}
