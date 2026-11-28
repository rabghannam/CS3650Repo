/**
 *  Tests for the Temporal Echo System (GDD section 4.3)
 *  Author: Rabi Ghannam
*/
/**
 *  RED-GREEN-REFACTOR: these tests were written first (RED) against the
 *  feature spec, then the minimum implementation was added in
 *  src/EchoRecording.cs, src/EchoRecorder.cs and src/Echo.cs (GREEN).
 *
 *  Requirements under test (GDD 4.3 - Temporal Echo System):
 *    1. Recording: hold Shift to record player movement, max 3.0 sec.
 *    2. Storage: one echo recording stored; a new recording overwrites it.
 *    3. Deployment: spawn echo that replays the recorded path; 15.0 sec cooldown.
 *    4. Echo movement: replicates the recorded path with 100% accuracy.
 *    5. Echo firing: fires in sync with the player (5 shots/sec), damage
 *       multiplier 1.0x (upgradable to 2.0x).
 *    6. Echo collection: auto-collects drops within 32 units.
 *    7. Echo decoy: absorbs 1 hit.
 *
 *  All tests live in the single "Tests" assembly so the whole game project
 *  runs from one Test Runner.
 */
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EchoTests
{
    private EchoRecorder _recorder;
    private Echo _echo;
    private Player _player;
    private Game _game;
    private GameObject _bulletPrefab;
    private readonly List<GameObject> _spawned = new List<GameObject>();
    private readonly List<Bullet> _bullets = new List<Bullet>();

    private const float EPSILON = 0.001f;

    [SetUp]
    public void SetUp()
    {
        // Start each test from a clean persisted-settings state.
        DeleteProgressKeys();
    }

    [TearDown]
    public void TearDown()
    {
        // Echo bullets spawned during tests.
        foreach (Bullet b in _bullets)
            if (b != null) Object.DestroyImmediate(b.gameObject);
        _bullets.Clear();

        if (_recorder != null) Object.DestroyImmediate(_recorder.gameObject);
        if (_echo != null) Object.DestroyImmediate(_echo.gameObject);

        // Echoes not tracked by the helpers (e.g. re-deploy after cooldown).
        foreach (Echo leftoverEcho in Object.FindObjectsOfType<Echo>())
            if (leftoverEcho != null) Object.DestroyImmediate(leftoverEcho.gameObject);
        if (_player != null) Object.DestroyImmediate(_player.gameObject);
        if (_game != null) Object.DestroyImmediate(_game.gameObject);
        if (_bulletPrefab != null) Object.DestroyImmediate(_bulletPrefab);

        foreach (GameObject obj in _spawned)
            if (obj != null) Object.DestroyImmediate(obj);
        _spawned.Clear();
        _recorder = null;
        _echo = null;
        _player = null;
        _game = null;
        _bulletPrefab = null;

        // Remove auto-created singletons so tests stay independent of one another.
        ProgressManager leftoverProgress = Object.FindObjectOfType<ProgressManager>();
        if (leftoverProgress != null) Object.DestroyImmediate(leftoverProgress.gameObject);

        DeleteProgressKeys();
    }

    private static void DeleteProgressKeys()
    {
        PlayerPrefs.DeleteKey("Progress_Money");
        PlayerPrefs.DeleteKey("Progress_MaxHealthLevel");
        PlayerPrefs.DeleteKey("Progress_FireRateLevel");
        PlayerPrefs.DeleteKey("Progress_BaseDamageLevel");
        PlayerPrefs.Save();
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    private EchoRecorder CreateRecorder()
    {
        GameObject obj = new GameObject("TestEchoRecorder");
        _spawned.Add(obj);
        _recorder = obj.AddComponent<EchoRecorder>();
        return _recorder;
    }

    private Player CreatePlayer()
    {
        GameObject obj = new GameObject("TestPlayer");
        _spawned.Add(obj);
        _player = obj.AddComponent<Player>();
        return _player;
    }

    private Game CreateGame(Player player)
    {
        GameObject obj = new GameObject("TestGame");
        _spawned.Add(obj);
        _game = obj.AddComponent<Game>();
        _game.player = player;
        return _game;
    }

    private GameObject CreateBulletPrefab()
    {
        GameObject obj = new GameObject("TestBulletPrefab");
        _spawned.Add(obj);
        obj.AddComponent<Bullet>();
        _bulletPrefab = obj;
        return obj;
    }

    private Powerup CreatePowerup(Vector2 position)
    {
        GameObject obj = new GameObject("TestPowerup");
        _spawned.Add(obj);
        Powerup powerup = obj.AddComponent<Powerup>();
        powerup.position = position;
        powerup.powerupType = Powerup.PowerupType.RapidFire;
        return powerup;
    }

    private static EchoRecording MakeTriangleRecording()
    {
        // A 2-second path with two legs: (0,0) -> (100,0) -> (100,100).
        EchoRecording recording = new EchoRecording();
        recording.AddSample(0f, new Vector2(0f, 0f));
        recording.AddSample(1f, new Vector2(100f, 0f));
        recording.AddSample(2f, new Vector2(100f, 100f));
        return recording;
    }

    private Echo CreateEcho(EchoRecording recording)
    {
        GameObject obj = new GameObject("TestEcho");
        _spawned.Add(obj);
        _echo = obj.AddComponent<Echo>();
        _echo.recording = recording;
        return _echo;
    }

    // ------------------------------------------------------------------
    // Requirement 1: Recording caps at 3.0 seconds
    // ------------------------------------------------------------------

    [Test]
    public void Recording_StopsAutomaticallyAtMaxDuration_ThreeSeconds()
    {
        EchoRecorder recorder = CreateRecorder();

        // Hold "Shift" for 12 ticks of 0.5s = 6 seconds of attempted recording.
        for (int i = 0; i < 12; i++)
            recorder.Tick(0.5f, new Vector2(i, 0f), recordHeld: true);

        Assert.IsFalse(recorder.IsRecording, "recording must stop on its own at the 3.0s cap");
        Assert.IsTrue(recorder.HasStoredRecording, "the capped recording must be stored");
        Assert.AreEqual(EchoRecorder.MAX_RECORD_DURATION, recorder.StoredRecording.Duration, EPSILON);
    }

    // ------------------------------------------------------------------
    // Requirement 2: One storage slot; a new recording overwrites the old one
    // ------------------------------------------------------------------

    [Test]
    public void Recording_NewRecordingOverwritesStoredOne()
    {
        EchoRecorder recorder = CreateRecorder();

        // First recording: 4 ticks of 0.25s = 1 second.
        for (int i = 0; i < 4; i++)
            recorder.Tick(0.25f, new Vector2(i, 0f), recordHeld: true);
        recorder.Tick(0.1f, Vector2.zero, recordHeld: false);
        float firstDuration = recorder.StoredRecording.Duration;

        // Second recording: 2 ticks of 0.25s = 0.5 second (shorter on purpose).
        for (int i = 0; i < 2; i++)
            recorder.Tick(0.25f, new Vector2(0f, i), recordHeld: true);
        recorder.Tick(0.1f, Vector2.zero, recordHeld: false);

        Assert.AreEqual(1f, firstDuration, EPSILON, "first recording should be 1 second");
        Assert.AreEqual(0.5f, recorder.StoredRecording.Duration, EPSILON,
            "the newer (shorter) recording must overwrite the stored one");
    }

    // ------------------------------------------------------------------
    // Requirement 3: Deployment needs a stored recording and a cooldown
    // ------------------------------------------------------------------

    [Test]
    public void Deploy_WithoutStoredRecording_ReturnsNull()
    {
        EchoRecorder recorder = CreateRecorder();

        Assert.IsNull(recorder.Deploy(), "cannot deploy an echo without a recording");
    }

    [Test]
    public void Deploy_SetsFifteenSecondCooldown()
    {
        EchoRecorder recorder = CreateRecorder();
        Recorder.RecordForSeconds(recorder, 1f);

        Echo echo = recorder.Deploy();

        Assert.IsNotNull(echo, "deploying with a stored recording must spawn an echo");
        Assert.IsFalse(recorder.CanDeploy, "echo cannot be redeployed immediately");
        Assert.AreEqual(EchoRecorder.DEPLOY_COOLDOWN, recorder.CooldownRemaining, EPSILON);
    }

    [Test]
    public void Deploy_CooldownUnlocksAfterFifteenSeconds_RegardlessOfFrameRate()
    {
        // Same total time, two different frame rates: the cooldown must be
        // frame-rate independent (measured in seconds, not frames).
        EchoRecorder bigFrames = CreateRecorder();
        Recorder.RecordForSeconds(bigFrames, 0.5f);
        Assert.IsNotNull(bigFrames.Deploy());
        bigFrames.Tick(10f, Vector2.zero, recordHeld: false);
        bigFrames.Tick(5f, Vector2.zero, recordHeld: false);

        EchoRecorder smallFrames = CreateRecorder();
        Recorder.RecordForSeconds(smallFrames, 0.5f);
        Assert.IsNotNull(smallFrames.Deploy());
        for (int i = 0; i < 60; i++) // 60 frames of 0.25s = 15 seconds
            smallFrames.Tick(0.25f, Vector2.zero, recordHeld: false);

        Assert.IsTrue(bigFrames.CanDeploy, "cooldown must expire after 15s (1s frames)");
        Assert.IsTrue(smallFrames.CanDeploy, "cooldown must expire after 15s (0.25s frames)");
    }

    // ------------------------------------------------------------------
    // Requirement 4: Echo replays the recorded path with 100% accuracy
    // ------------------------------------------------------------------

    [Test]
    public void Echo_ReplaysRecordedPathWithFullAccuracy()
    {
        Echo echo = CreateEcho(MakeTriangleRecording());

        echo.CustomUpdate(0.5f); // t = 0.5 -> halfway through leg 1
        Assert.AreEqual(50f, echo.position.x, EPSILON);
        Assert.AreEqual(0f, echo.position.y, EPSILON);

        echo.CustomUpdate(0.5f); // t = 1.0 -> end of leg 1
        Assert.AreEqual(100f, echo.position.x, EPSILON);
        Assert.AreEqual(0f, echo.position.y, EPSILON);

        echo.CustomUpdate(1f); // t = 2.0 -> end of leg 2, playback finished
        Assert.AreEqual(100f, echo.position.x, EPSILON);
        Assert.AreEqual(100f, echo.position.y, EPSILON);
        Assert.IsTrue(echo.playbackFinished, "playback must finish at the end of the recording");

        Vector2 finalPosition = echo.position;
        echo.CustomUpdate(1f); // finished echo must not move or throw
        Assert.AreEqual(finalPosition, echo.position);
    }

    // ------------------------------------------------------------------
    // Requirement 5: Echo fires in sync with the player, with a damage multiplier
    // ------------------------------------------------------------------

    [Test]
    public void Echo_FiresInSyncWithPlayer_WithDamageMultiplier()
    {
        Player player = CreatePlayer();
        Game game = CreateGame(player);
        CreateBulletPrefab();

        Echo echo = CreateEcho(MakeTriangleRecording());
        echo.game = game;
        echo.player = player;
        echo.bulletPrefab = _bulletPrefab;
        echo.damageMultiplier = 2f; // GDD: Echo Damage upgrade maxes at 2.0x

        // Player fire rate is 5 shots/sec -> echo fires at 0.2s intervals too.
        for (int i = 0; i < 5; i++)
        {
            _bullets.AddRange(game.bullets);
            game.bullets.Clear();
            echo.CustomUpdate(0.2f);
        }
        _bullets.AddRange(game.bullets);

        Assert.AreEqual(5, _bullets.Count, "echo must fire 5 shots in 1 second, like the player");
        foreach (Bullet bullet in _bullets)
        {
            Assert.AreEqual(Bullet.Owner.Player, bullet.owner, "echo bullets belong to the player");
            Assert.AreEqual(2, bullet.getDamage(), "damage must be base damage (1) x 2.0x multiplier");
        }
    }

    // ------------------------------------------------------------------
    // Requirement 6: Echo auto-collects drops within 32 units
    // ------------------------------------------------------------------

    [Test]
    public void Echo_CollectsDropsWithinThirtyTwoUnits()
    {
        Player player = CreatePlayer();
        Game game = CreateGame(player);

        Powerup nearbyDrop = CreatePowerup(new Vector2(20f, 0f));   // 10 units from the echo at t=0.1
        Powerup farDrop = CreatePowerup(new Vector2(60f, 0f));      // 50 units from the echo: outside 32
        game.powerups.Add(nearbyDrop);
        game.powerups.Add(farDrop);

        Echo echo = CreateEcho(MakeTriangleRecording());
        echo.game = game;
        echo.player = player;

        echo.CustomUpdate(0.1f); // echo sits near (0,0) at t = 0.1

        Assert.IsFalse(game.powerups.Contains(nearbyDrop),
            "drop within 32 units must be collected by the echo");
        Assert.AreEqual(2f, player.GetFireRateMultiplier(),
            "collected RapidFire drop must apply its effect to the player");
        Assert.IsTrue(game.powerups.Contains(farDrop),
            "drop beyond 32 units must be left alone");
    }

    // ------------------------------------------------------------------
    // Requirement 7: Echo decoy absorbs exactly one hit
    // ------------------------------------------------------------------

    [Test]
    public void Echo_DecoyAbsorbsExactlyOneHit()
    {
        Echo echo = CreateEcho(MakeTriangleRecording());

        Assert.AreEqual(1, echo.decoyHits, "echo starts with 1 decoy use (GDD 4.3)");
        Assert.IsTrue(echo.TryAbsorbHit(), "first hit must be absorbed");
        Assert.IsFalse(echo.TryAbsorbHit(), "a used-up echo must not absorb more hits");
    }

    // ------------------------------------------------------------------
    // Asynchronous tests (one editor frame per step, like real gameplay)
    // ------------------------------------------------------------------

    [UnityTest]
    public IEnumerator Recording_OverFrames_StoresPlayableRecording()
    {
        EchoRecorder recorder = CreateRecorder();

        // Hold "record" while the player "moves" right, one frame at a time
        // (fixed 0.05 s steps so the test does not depend on Time.deltaTime).
        for (int frame = 0; frame < 40; frame++)
        {
            recorder.Tick(0.05f, new Vector2(frame * 5f, 0f), recordHeld: true);
            yield return null;
        }
        recorder.Tick(0.05f, Vector2.zero, recordHeld: false);
        yield return null;

        Assert.IsTrue(recorder.HasStoredRecording, "holding record across frames must store a recording");
        Assert.Greater(recorder.StoredRecording.Duration, 0f, "stored recording must have duration");
        Assert.LessOrEqual(recorder.StoredRecording.Duration, EchoRecorder.MAX_RECORD_DURATION + EPSILON,
            "stored recording must respect the 3.0s cap");
    }

    [UnityTest]
    public IEnumerator Echo_PlaybackFinishesAfterRecordingDuration_OverFrames()
    {
        Echo echo = CreateEcho(MakeTriangleRecording());

        // Step the echo once per frame with a fixed step; 2s / 0.1s = 20 steps.
        for (int frame = 0; frame < 25 && !echo.playbackFinished; frame++)
        {
            echo.CustomUpdate(0.1f);
            yield return null;
        }

        Assert.IsTrue(echo.playbackFinished, "echo playback must finish after the recording ends");
        Assert.AreEqual(100f, echo.position.x, EPSILON, "echo must rest at the final recorded position");
        Assert.AreEqual(100f, echo.position.y, EPSILON, "echo must rest at the final recorded position");
    }

    [UnityTest]
    public IEnumerator Recorder_CooldownUnlocksOverFrames_AfterDeploy()
    {
        EchoRecorder recorder = CreateRecorder();
        Recorder.RecordForSeconds(recorder, 0.5f);
        Assert.IsNotNull(recorder.Deploy());

        // 40 frames of 0.5s = 20 seconds > 15s cooldown.
        for (int frame = 0; frame < 40; frame++)
        {
            recorder.Tick(0.5f, Vector2.zero, recordHeld: false);
            yield return null;
        }

        Assert.IsTrue(recorder.CanDeploy, "cooldown must unlock after 15 real seconds of frames");
        Assert.IsNotNull(recorder.Deploy(), "echo must be deployable again after the cooldown");
    }

    /// <summary>Small helper shared by the tests above: simulates holding Shift.</summary>
    private static class Recorder
    {
        public static void RecordForSeconds(EchoRecorder recorder, float seconds, float tick = 0.25f)
        {
            int ticks = Mathf.CeilToInt(seconds / tick);
            for (int i = 0; i < ticks; i++)
                recorder.Tick(tick, new Vector2(i * 10f, 0f), recordHeld: true);
            recorder.Tick(tick, Vector2.zero, recordHeld: false); // release Shift
        }
    }
}
