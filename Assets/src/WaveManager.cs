using System;
using UnityEngine;

/// <summary>
/// US-Boss: wave progression and boss introduction.
/// - Boss appears exactly every BossWaveInterval waves (10, 20, 30, ...).
/// - Reaching a boss wave triggers a special boss announcement and spawns
///   the boss prefab (if assigned).
/// Both Play mode (automatic progression) and unit tests (AdvanceWave) work.
/// </summary>
public class WaveManager : MonoBehaviour
{
    /// <summary>Boss appears exactly every 10 waves.</summary>
    public const int BossWaveInterval = 10;

    /// <summary>Boss base health kept here too so the wave system can show the value.</summary>
    public const int BossBaseHealth = Boss.BaseHealth;

    /// <summary>How long the boss announcement stays on screen (seconds).</summary>
    public const float AnnouncementDuration = 3f;

    /// <summary>The special announcement shown when a boss wave starts.</summary>
    public const string BossAnnouncementText = "WARNING! A BOSS IS INCOMING!";

    [Header("Waves")]
    [Tooltip("Seconds each wave lasts (game time). 0 or less disables automatic progression.")]
    public float secondsPerWave = 20f;

    [Header("Boss spawning")]
    [Tooltip("Boss prefab spawned when a boss wave starts.")]
    public GameObject bossPrefab;

    [Header("References")]
    [Tooltip("Optional game reference used to register spawned bosses.")]
    public Game game;

    /// <summary>The wave currently in progress. Starts at 0; first AdvanceWave -> wave 1.</summary>
    public int currentWave { get; private set; }

    /// <summary>True while the boss announcement is being shown.</summary>
    public bool announcementActive { get; private set; }

    /// <summary>Raised when a boss wave starts (receives the wave number).</summary>
    public event Action<int> BossWaveReached;

    /// <summary>Raised when the special boss announcement starts (receives the text).</summary>
    public event Action<string> BossAnnounced;

    /// <summary>Raised when a boss instance is spawned (receives the instance).</summary>
    public event Action<GameObject> BossSpawned;

    private float waveTimer;
    private float announcementTimer;

    private void Update()
    {
        CustomUpdate(Time.deltaTime); // frame-rate independent
    }

    /// <summary>Frame-rate independent update; public so tests can drive it.</summary>
    public void CustomUpdate(float deltaTime)
    {
        if (announcementActive)
        {
            announcementTimer -= deltaTime;
            if (announcementTimer <= 0f)
                announcementActive = false;
        }

        // Automatic wave progression only while playing, so unit tests
        // control wave advancement explicitly.
        if (Application.isPlaying && secondsPerWave > 0f)
        {
            waveTimer += deltaTime;
            if (waveTimer >= secondsPerWave)
            {
                waveTimer -= secondsPerWave;
                AdvanceWave();
            }
        }
    }

    /// <summary>Starts the next wave, announcing and spawning a boss when needed.</summary>
    public void AdvanceWave()
    {
        currentWave++;

        if (IsBossWave(currentWave))
        {
            BossWaveReached?.Invoke(currentWave);
            AnnounceBoss();
            SpawnBoss();
        }
    }

    /// <summary>A wave is a boss wave exactly when it is a positive multiple of 10.</summary>
    public static bool IsBossWave(int wave)
    {
        return wave > 0 && wave % BossWaveInterval == 0;
    }

    /// <summary>Requirement: boss introduction includes a special announcement.</summary>
    public void AnnounceBoss()
    {
        announcementActive = true;
        announcementTimer = AnnouncementDuration;
        BossAnnounced?.Invoke(BossAnnouncementText);
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null) return;

        GameObject bossObject = Instantiate(bossPrefab, transform.position, Quaternion.identity);
        bossObject.name = "Boss";
        bossObject.SetActive(true);

        BossSpawned?.Invoke(bossObject);

        // Register with the game so collisions/HUD see the boss (mirrors EnemySpawner).
        Enemy bossEnemy = bossObject.GetComponent<Enemy>();
        if (bossEnemy != null && game != null)
        {
            game.enemies.Add(bossEnemy);
        }
    }
}
