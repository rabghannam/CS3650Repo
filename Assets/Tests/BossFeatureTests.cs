using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Unit tests for the Boss feature (US-Boss).
/// Requirements under test:
///   1. Boss appears exactly every 10 waves (wave 10, 20, 30, ...)
///   2. Boss has exactly 50 base health
///   3. Boss visual treatment is distinct from regular enemies
///   4. Boss has elite visual effects (larger, brighter, more prominent)
///   5. Boss introduction includes a special announcement
///
/// RED-GREEN-REFACTOR: these tests were written first (RED). The minimum
/// implementation lives in src/Boss.cs and src/WaveManager.cs (GREEN).
///
/// All tests live in the single "Tests" assembly so the whole game project
/// runs from one Test Runner.
/// </summary>
public class BossFeatureTests
{
    private Boss _boss;
    private WaveManager _waveManager;
    private GameObject _bossPrefabTemplate;
    private GameObject _regularEnemy;
    private readonly System.Collections.Generic.List<GameObject> _spawnedBosses =
        new System.Collections.Generic.List<GameObject>();

    [SetUp]
    public void SetUp()
    {
        // Keep editor PlayerPrefs clean: Enemy.Die() awards money to ProgressManager.
        DeleteProgressKeys();
    }

    [TearDown]
    public void TearDown()
    {
        if (_boss != null) Object.DestroyImmediate(_boss.gameObject);
        if (_waveManager != null) Object.DestroyImmediate(_waveManager.gameObject);
        if (_bossPrefabTemplate != null) Object.DestroyImmediate(_bossPrefabTemplate);
        if (_regularEnemy != null) Object.DestroyImmediate(_regularEnemy);
        foreach (GameObject spawned in _spawnedBosses)
            if (spawned != null) Object.DestroyImmediate(spawned);
        _spawnedBosses.Clear();
        _boss = null;
        _waveManager = null;
        _bossPrefabTemplate = null;
        _regularEnemy = null;

        // Remove the ProgressManager singleton that Enemy.Die() auto-creates,
        // so tests stay independent of one another.
        ProgressManager leftover = Object.FindObjectOfType<ProgressManager>();
        if (leftover != null) Object.DestroyImmediate(leftover.gameObject);

        DeleteProgressKeys();
    }

    private static void DeleteProgressKeys()
    {
        PlayerPrefs.DeleteKey("Progress_Money");
        PlayerPrefs.DeleteKey("Progress_MaxHealthLevel");
        PlayerPrefs.DeleteKey("Progress_FireRateLevel");
        PlayerPrefs.DeleteKey("Progress_BaseDamageLevel");
    }

    private Boss CreateBoss(Vector3? spawnPosition = null)
    {
        GameObject obj = new GameObject("TestBoss");
        obj.transform.position = spawnPosition ?? Vector3.zero;
        obj.AddComponent<SpriteRenderer>();
        _boss = obj.AddComponent<Boss>();
        // NOTE: Awake() does not run on AddComponent in Edit Mode tests,
        // so Enemy.spriteRenderer is wired manually.
        _boss.spriteRenderer = obj.GetComponent<SpriteRenderer>();
        return _boss;
    }

    private Enemy CreateRegularEnemy()
    {
        GameObject obj = new GameObject("TestRegularEnemy");
        obj.transform.position = Vector3.zero;
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.color = new Color(0.14440194f, 0.5566038f, 0.21884821f, 1f); // regular green
        Enemy enemy = obj.AddComponent<Enemy>();
        enemy.spriteRenderer = renderer;          // Awake() does not run in Edit Mode tests
        enemy.originalColor = renderer.color;
        _regularEnemy = obj;
        return enemy;
    }

    private WaveManager CreateWaveManager()
    {
        GameObject obj = new GameObject("TestWaveManager");
        _waveManager = obj.AddComponent<WaveManager>();
        return _waveManager;
    }

    private static float Luminance(Color c)
    {
        return c.r * 0.299f + c.g * 0.587f + c.b * 0.114f;
    }

    // ------------------------------------------------------------------
    // Requirement 1: Boss appears exactly every 10 waves (10, 20, 30, ...)
    // ------------------------------------------------------------------

    [TestCase(10, true)]
    [TestCase(20, true)]
    [TestCase(30, true)]
    [TestCase(100, true)]
    [TestCase(0, false)]
    [TestCase(1, false)]
    [TestCase(9, false)]
    [TestCase(11, false)]
    [TestCase(19, false)]
    [TestCase(21, false)]
    [TestCase(27, false)]
    public void IsBossWave_TrueExactlyAtWaveMultiplesOfTen(int wave, bool expected)
    {
        Assert.AreEqual(expected, WaveManager.IsBossWave(wave),
            $"Wave {wave} {(expected ? "should" : "should not")} be a boss wave");
    }

    [Test]
    public void WavesBeforeFirstBossWave_NeverAnnounceOrSpawnBoss()
    {
        WaveManager waveManager = CreateWaveManager();
        int announcedCount = 0;
        int spawnedCount = 0;
        waveManager.BossAnnounced += _ => announcedCount++;
        waveManager.BossSpawned += _ => spawnedCount++;

        for (int i = 0; i < WaveManager.BossWaveInterval - 1; i++)
            waveManager.AdvanceWave();

        Assert.AreEqual(WaveManager.BossWaveInterval - 1, waveManager.currentWave);
        Assert.AreEqual(0, announcedCount, "No boss announcement before wave 10");
        Assert.AreEqual(0, spawnedCount, "No boss spawned before wave 10");
    }

    // ------------------------------------------------------------------
    // Requirement 2: Boss has exactly 50 base health
    // ------------------------------------------------------------------

    [Test]
    public void Boss_Initialized_HasExactlyFiftyBaseHealth()
    {
        Boss boss = CreateBoss();
        boss.InitializeBoss();

        Assert.AreEqual(50, boss.health, "Boss base health must be exactly 50");
        Assert.AreEqual(50, Boss.BaseHealth, "Boss.BaseHealth constant must be 50");
        Assert.AreEqual(50, WaveManager.BossBaseHealth, "WaveManager.BossBaseHealth must match Boss health");
    }

    // ------------------------------------------------------------------
    // Requirements 3 & 4: distinct + elite visuals (larger, brighter)
    // ------------------------------------------------------------------

    [Test]
    public void Boss_EliteVisuals_AreLargerAndBrighterThanRegularEnemy()
    {
        Enemy regular = CreateRegularEnemy();
        Boss boss = CreateBoss();

        boss.InitializeBoss();

        // Distinct treatment: elite red/gold vs regular green
        Assert.AreNotEqual(regular.spriteRenderer.color, boss.spriteRenderer.color,
            "Boss color must differ from regular enemy color");

        // Larger: scale factor must exceed regular enemy scale (1x)
        Assert.Greater(boss.transform.localScale.x, regular.transform.localScale.x,
            "Boss must be larger than a regular enemy");
        Assert.GreaterOrEqual(boss.transform.localScale.x, Boss.EliteScale,
            "Boss scale must reach the elite scale constant");

        // Brighter: elite color has a strictly higher luminance than regular green
        Assert.Greater(Luminance(boss.spriteRenderer.color), Luminance(regular.spriteRenderer.color),
            "Boss must appear brighter than a regular enemy");
        Assert.IsTrue(boss.IsEliteActive, "Boss elite visual state must be active");
    }

    // ------------------------------------------------------------------
    // Requirement 5: special announcement on boss introduction
    // ------------------------------------------------------------------

    [Test]
    public void BossWave_Starts_AnnouncesBossIntroductionExactlyOnce()
    {
        WaveManager waveManager = CreateWaveManager();
        int announcedCount = 0;
        string announcedText = null;
        waveManager.BossAnnounced += text => { announcedCount++; announcedText = text; };

        for (int i = 0; i < WaveManager.BossWaveInterval; i++)
            waveManager.AdvanceWave(); // waves 1..10; wave 10 is the first boss wave

        Assert.AreEqual(1, announcedCount, "Boss announcement fired exactly once in waves 1..10");
        Assert.IsFalse(string.IsNullOrEmpty(announcedText), "Announcement text must not be empty");
        StringAssert.Contains("BOSS", announcedText.ToUpper(), "Announcement must mention the boss");
        Assert.IsTrue(waveManager.announcementActive, "Announcement must be active right after a boss wave starts");
    }

    // ------------------------------------------------------------------
    // Asynchronous tests (at least two required by the assignment)
    // ------------------------------------------------------------------

    /// <summary>Asynchronous: boss survives frame-by-frame damage until 50 damage kills it.</summary>
    [UnityTest]
    public IEnumerator Boss_DiesOnlyAfterFiftyDamage_SpreadAcrossFrames()
    {
        Boss boss = CreateBoss();
        boss.InitializeBoss();

        for (int damageDone = 0; damageDone < Boss.BaseHealth; damageDone++)
        {
            boss.takeDamage(1);
            if (damageDone < Boss.BaseHealth - 1)
            {
                Assert.IsTrue(boss.isAlive, $"Boss must survive after taking {damageDone + 1} damage");
                Assert.Greater(boss.health, 0);
                yield return null; // wait one frame (asynchronous step)
            }
        }

        Assert.IsFalse(boss.isAlive, "Boss must die after exactly 50 total damage");
        Assert.LessOrEqual(boss.health, 0);
    }

    /// <summary>Asynchronous: boss announcement appears at wave 10 and fades out after its duration.</summary>
    [UnityTest]
    public IEnumerator BossAnnouncement_ShowsThenFadesOutOverFrames()
    {
        WaveManager waveManager = CreateWaveManager();

        for (int i = 0; i < WaveManager.BossWaveInterval; i++)
        {
            waveManager.AdvanceWave();
            yield return null; // wait one frame (asynchronous step)
        }

        Assert.IsTrue(waveManager.announcementActive, "Announcement active at boss wave 10");

        float elapsed = 0f;
        while (waveManager.announcementActive && elapsed < WaveManager.AnnouncementDuration + 1f)
        {
            waveManager.CustomUpdate(0.25f); // simulate quarter-second steps
            elapsed += 0.25f;
            yield return null;
        }

        Assert.IsFalse(waveManager.announcementActive,
            "Announcement must fade out after its duration");
        Assert.GreaterOrEqual(elapsed, WaveManager.AnnouncementDuration,
            "Announcement must last at least the full announcement duration");
    }

    /// <summary>Asynchronous: waves 10 and 20 each spawn exactly one boss instance.</summary>
    [UnityTest]
    public IEnumerator BossWave_SpawnsExactlyOneBossPerBossWave()
    {
        WaveManager waveManager = CreateWaveManager();

        // Hidden template acting as the assigned boss prefab
        _bossPrefabTemplate = new GameObject("BossPrefabTemplate");
        _bossPrefabTemplate.AddComponent<SpriteRenderer>();
        _bossPrefabTemplate.AddComponent<Boss>();
        _bossPrefabTemplate.SetActive(false);

        waveManager.bossPrefab = _bossPrefabTemplate;
        int spawnCount = 0;
        waveManager.BossSpawned += spawnedBoss =>
        {
            _spawnedBosses.Add(spawnedBoss);
            spawnCount++;
        };

        for (int i = 0; i < WaveManager.BossWaveInterval * 2; i++) // waves 1..20
        {
            waveManager.AdvanceWave();
            yield return null;
        }

        Assert.AreEqual(2, spawnCount,
            "Exactly one boss must spawn per boss wave (waves 10 and 20 only)");
    }
}
