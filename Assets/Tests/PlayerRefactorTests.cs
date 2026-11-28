/**
 *  Characterization tests for the Player feature (pre-refactor).
 *  Author: Rabi Ghannam
*/
/**
 *  TDD REFACTOR PHASE: these tests were written against the CURRENT
 *  Player.cs behaviour BEFORE any refactoring. They must pass before the
 *  refactor and must keep passing unchanged afterwards, proving the
 *  refactor preserved functionality (Red-Green-Refactor, refactor step).
 *
 *  Behaviour pinned down (from the current Player.cs implementation):
 *    1. Fresh player: 50 max health, Alive state, all powerup multipliers at 1.
 *    2. takeDamage: instant, clamped at 0, triggers 1 s i-frames, extra hits
 *       during i-frames are ignored, death at 0 notifies the Game.
 *    3. Shield powerup blocks all damage; expiry returns the player to Alive.
 *    4. Powerup timers (rapid fire, damage amplifier, magnet, shield) tick
 *       down and reset their effect when they expire.
 *    5. Movement follows the scripted input direction at BASE_SPEED with
 *       normalized diagonals; a dying player ignores input.
 *    6. SpawnBullet applies the ProgressManager base-damage upgrade.
 *
 *  Tests target the public API wherever possible. The two behaviours only
 *  reachable through private members (the powerup timer tick and the
 *  damage-multiplier state) use a reflection helper that also binds to the
 *  extracted powerup-state object planned for the refactor, so the file
 *  keeps passing without modification after refactoring.
 */
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

public class PlayerRefactorTests
{
    private Player _player;
    private HUD _hud;
    private Game _game;
    private GameObject _bulletPrefab;
    private readonly List<GameObject> _spawned = new List<GameObject>();

    [SetUp]
    public void SetUp()
    {
        // Start each test from a clean persisted-progress state.
        DeleteProgressKeys();
    }

    [TearDown]
    public void TearDown()
    {
        // Bullets spawned by the player during tests (must go before _game is destroyed).
        if (_game != null)
            foreach (Bullet b in _game.bullets)
                if (b != null) Object.DestroyImmediate(b.gameObject);

        if (_bulletPrefab != null) Object.DestroyImmediate(_bulletPrefab);
        if (_player != null) Object.DestroyImmediate(_player.gameObject);
        if (_hud != null) Object.DestroyImmediate(_hud.gameObject);
        if (_game != null) Object.DestroyImmediate(_game.gameObject);

        foreach (GameObject obj in _spawned)
            if (obj != null) Object.DestroyImmediate(obj);
        _spawned.Clear();
        _player = null;
        _hud = null;
        _game = null;
        _bulletPrefab = null;

        // Remove auto-created singletons so tests stay independent of one another.
        ProgressManager leftoverProgress = Object.FindObjectOfType<ProgressManager>();
        if (leftoverProgress != null) Object.DestroyImmediate(leftoverProgress.gameObject);
        SettingsManager leftoverSettings = Object.FindObjectOfType<SettingsManager>();
        if (leftoverSettings != null) Object.DestroyImmediate(leftoverSettings.gameObject);

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
    // Helpers (same conventions as the passing InvincibilityTests)
    // ------------------------------------------------------------------

    private Player CreatePlayer(int health = Player.BASE_HEALTH)
    {
        GameObject obj = new GameObject("TestPlayer");
        _spawned.Add(obj);
        _player = obj.AddComponent<Player>();
        _player.health = health;
        _player.maxHealth = Player.BASE_HEALTH;

        _hud = CreateHud();
        _player.hud = _hud;
        _player.game = CreateGame();
        _player.input = obj.AddComponent<ScriptedInput>();
        return _player;
    }

    private HUD CreateHud()
    {
        GameObject obj = new GameObject("TestHUD");
        _spawned.Add(obj);
        _hud = obj.AddComponent<HUD>();
        _hud.flashOverlay = obj.AddComponent<Image>();
        return _hud;
    }

    private Game CreateGame()
    {
        GameObject obj = new GameObject("TestGame");
        _spawned.Add(obj);
        _game = obj.AddComponent<Game>();
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

    /// <summary>Scripted input that bypasses the real keyboard (edit-mode tests cannot press keys).</summary>
    private class ScriptedInput : InputHandler
    {
        public Vector2 direction;
        public override InputHandler.InputState readKeyboard()
        {
            return new InputHandler.InputState { moveDirection = direction };
        }
    }

    /// <summary>
    /// Advances the powerup timers by deltaTime. Before the refactor the tick
    /// lives in Player.UpdatePowerupTimers(float); after the planned extraction
    /// it may live on the powerup-state object held by the player, so try both.
    /// </summary>
    private static void TickPowerupTimers(Player player, float deltaTime)
    {
        MethodInfo onPlayer = player.GetType().GetMethod("UpdatePowerupTimers",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
            null, new[] { typeof(float) }, null);
        if (onPlayer != null)
        {
            onPlayer.Invoke(player, new object[] { deltaTime });
            return;
        }

        // Look for the tick on any plain (non-UnityEngine.Object) field object.
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        foreach (FieldInfo field in player.GetType().GetFields(flags))
        {
            object value = field.GetValue(player);
            if (value == null || value is Object) continue; // skip components/assets
            MethodInfo tick = value.GetType().GetMethod("UpdatePowerupTimers", flags,
                null, new[] { typeof(float) }, null)
                ?? value.GetType().GetMethod("TickTimers", flags,
                    null, new[] { typeof(float) }, null)
                ?? value.GetType().GetMethod("Tick", flags,
                    null, new[] { typeof(float) }, null);
            if (tick != null)
            {
                tick.Invoke(value, new object[] { deltaTime });
                return;
            }
        }

        Assert.Fail("Could not find the powerup timer tick on Player or its state object");
    }

    /// <summary>Reads a private field by name from the player (or its state object).</summary>
    private static T GetPrivateField<T>(Player player, string name)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        FieldInfo field = player.GetType().GetField(name, flags);
        if (field != null) return (T)field.GetValue(player);

        foreach (FieldInfo candidate in player.GetType().GetFields(flags))
        {
            object value = candidate.GetValue(player);
            if (value == null || value is Object) continue;
            FieldInfo inner = value.GetType().GetField(name, flags);
            if (inner != null) return (T)inner.GetValue(value);
        }
        Assert.Fail("Field '" + name + "' not found on Player or its state object");
        return default;
    }

    // ------------------------------------------------------------------
    // 1. Fresh player state
    // ------------------------------------------------------------------

    [Test]
    public void FreshPlayer_StartsAliveAtFullHealth_WithNeutralPowerups()
    {
        Player player = CreatePlayer();

        Assert.AreEqual(Player.BASE_HEALTH, player.health, "player must start at base health");
        Assert.AreEqual(50, Player.BASE_HEALTH, "BASE_HEALTH constant must stay 50");
        Assert.AreEqual(Player.BASE_HEALTH, player.maxHealth, "max health must start at base");
        Assert.AreEqual(Player.PlayerState.Alive, player.state, "player must start Alive");
        Assert.AreEqual(1f, player.GetFireRateMultiplier(), "fire rate multiplier must start neutral");
        Assert.IsFalse(player.HasMagnet(), "magnet must start inactive");

        float range, speed;
        player.GetMagnetParams(out range, out speed);
        Assert.AreEqual(200f, range, "default magnet range must stay 200");
        Assert.AreEqual(300f, speed, "default magnet speed must stay 300");
    }

    [Test]
    public void PlayerConstants_KeepTheirDocumentedValues()
    {
        Assert.AreEqual(300f, Player.BASE_SPEED, "BASE_SPEED must stay 300 units/sec (GDD 4.1)");
        Assert.AreEqual(5f, Player.FIRE_RATE, "FIRE_RATE must stay 5 shots/sec (GDD 4.2)");
        Assert.AreEqual(1f, Player.IFRAME_DURATION, "IFRAME_DURATION must stay 1 second (US-009)");
    }

    // ------------------------------------------------------------------
    // 2. takeDamage: instant, clamped, i-frames, death
    // ------------------------------------------------------------------

    [Test]
    public void TakeDamage_AppliesInstantly_AndEntersOneSecondInvincibility()
    {
        Player player = CreatePlayer();

        player.takeDamage(1);

        Assert.AreEqual(Player.BASE_HEALTH - 1, player.health, "damage must apply instantly");
        Assert.AreEqual(Player.PlayerState.Invulnerable, player.state, "hit must enter i-frames");
        Assert.AreEqual(Player.IFRAME_DURATION, player.iFrameTimer, "i-frame timer must start at 1 second");
    }

    [Test]
    public void TakeDamage_DuringInvincibility_IsIgnored()
    {
        Player player = CreatePlayer();
        player.takeDamage(1);
        int healthAfterFirstHit = player.health;

        player.takeDamage(1);
        player.takeDamage(1);

        Assert.AreEqual(healthAfterFirstHit, player.health, "hits during i-frames must not reduce health");
        Assert.AreEqual(Player.IFRAME_DURATION, player.iFrameTimer, "hits during i-frames must not extend the timer");
    }

    [Test]
    public void TakeDamage_ClampsAtZero_AndNotifiesGameOver()
    {
        Player player = CreatePlayer(health: 1);

        player.takeDamage(5);

        Assert.AreEqual(0, player.health, "health must clamp at 0, never go negative");
        Assert.AreEqual(Player.PlayerState.Dying, player.state, "death must move the player to Dying");
        Assert.IsTrue(_game.isGameOver, "death must notify the Game (US-008)");
    }

    [Test]
    public void TakeDamage_WhileDying_IsIgnored()
    {
        Player player = CreatePlayer(health: 1);
        player.takeDamage(5);

        player.takeDamage(10);

        Assert.AreEqual(0, player.health, "a dying player cannot lose more health");
        Assert.AreEqual(Player.PlayerState.Dying, player.state, "state must remain Dying");
    }

    // ------------------------------------------------------------------
    // 3. Shield powerup blocks damage, then expires
    // ------------------------------------------------------------------

    [Test]
    public void Shield_BlocksAllDamage_WhileActive()
    {
        Player player = CreatePlayer();

        player.ActivateShield(5f);
        player.takeDamage(10);

        Assert.AreEqual(Player.BASE_HEALTH, player.health, "shield must block all damage");
        Assert.AreEqual(Player.PlayerState.Invulnerable, player.state, "shield activates i-frames");
    }

    [Test]
    public void Shield_Expiry_ReturnsPlayerToAlive_AndStopsBlocking()
    {
        Player player = CreatePlayer();
        player.ActivateShield(5f);

        TickPowerupTimers(player, 5f);   // shield timer runs out
        player.CustomUpdate(5f);         // i-frame timer runs out

        Assert.IsFalse(GetPrivateField<bool>(player, "hasShield"), "shield flag must reset on expiry");
        Assert.AreEqual(Player.PlayerState.Alive, player.state, "player must return to Alive after shield expiry");

        player.takeDamage(1);
        Assert.AreEqual(Player.BASE_HEALTH - 1, player.health, "after expiry damage must apply again");
    }

    // ------------------------------------------------------------------
    // 4. Powerup timers tick down and reset on expiry
    // ------------------------------------------------------------------

    [Test]
    public void RapidFire_ActiveUntilItsTimerExpires()
    {
        Player player = CreatePlayer();

        player.ActivateRapidFire(2.5f, 1f);
        Assert.AreEqual(2.5f, player.GetFireRateMultiplier(), "rapid fire multiplier must apply immediately");

        TickPowerupTimers(player, 0.99f);
        Assert.AreEqual(2.5f, player.GetFireRateMultiplier(), "multiplier must survive until the full duration");

        TickPowerupTimers(player, 0.01f);
        Assert.AreEqual(1f, player.GetFireRateMultiplier(), "multiplier must reset to 1 on expiry");
    }

    [Test]
    public void DamageAmplifier_ActiveUntilItsTimerExpires()
    {
        Player player = CreatePlayer();

        player.ActivateDamageAmplifier(3f, 2f);
        Assert.AreEqual(3f, GetPrivateField<float>(player, "damageMultiplier"),
            "damage amplifier must apply its multiplier immediately");

        TickPowerupTimers(player, 2f);
        Assert.AreEqual(1f, GetPrivateField<float>(player, "damageMultiplier"),
            "damage multiplier must reset to 1 on expiry");
    }

    [Test]
    public void Magnet_ActiveUntilItsTimerExpires()
    {
        Player player = CreatePlayer();

        player.ActivateMagnet(150f, 250f, 1f);
        Assert.IsTrue(player.HasMagnet(), "magnet must be active right after activation");
        float range, speed;
        player.GetMagnetParams(out range, out speed);
        Assert.AreEqual(150f, range, "magnet range must come from the powerup");
        Assert.AreEqual(250f, speed, "magnet speed must come from the powerup");

        TickPowerupTimers(player, 1f);
        Assert.IsFalse(player.HasMagnet(), "magnet must deactivate on expiry");
    }

    // ------------------------------------------------------------------
    // 5. Movement follows input, dies ignore input
    // ------------------------------------------------------------------

    [Test]
    public void Movement_FollowsScriptedInputDirection_AtBaseSpeed()
    {
        Player player = CreatePlayer();

        ((ScriptedInput)player.input).direction = Vector2.right;
        player.CustomUpdate(0.1f);
        FieldInfo velocityField = typeof(Player).GetField("velocity",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Vector2 velocity = (Vector2)velocityField.GetValue(player);
        Assert.AreEqual(new Vector2(Player.BASE_SPEED, 0f), velocity,
            "input right must set velocity to BASE_SPEED on x, zero on y");

        ((ScriptedInput)player.input).direction = new Vector2(1f, 1f);
        player.CustomUpdate(0.1f);
        velocity = (Vector2)velocityField.GetValue(player);
        float diagonal = Player.BASE_SPEED / Mathf.Sqrt(2f);
        Assert.AreEqual(diagonal, velocity.x, 0.01f, "diagonal input must be normalized");
        Assert.AreEqual(diagonal, velocity.y, 0.01f, "diagonal input must be normalized");
    }

    [Test]
    public void Movement_DyingPlayer_IgnoresInput()
    {
        Player player = CreatePlayer();
        player.state = Player.PlayerState.Dying;
        Vector2 positionBefore = player.position;

        ((ScriptedInput)player.input).direction = Vector2.right;
        player.CustomUpdate(0.1f);

        Assert.AreEqual(positionBefore, player.position, "a dying player must not move");
    }

    // ------------------------------------------------------------------
    // 6. SpawnBullet applies the ProgressManager base-damage upgrade
    // ------------------------------------------------------------------

    [Test]
    public void SpawnBullet_AppliesProgressManagerBaseDamage()
    {
        Player player = CreatePlayer();
        player.bulletPrefab = CreateBulletPrefab();

        MethodInfo spawn = player.GetType().GetMethod("SpawnBullet",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(spawn, "Player must keep a SpawnBullet entry point");
        spawn.Invoke(player, null);

        Assert.AreEqual(1, _game.bullets.Count, "spawning must register the bullet with the Game");
        Assert.AreEqual(1, _game.bullets[0].getDamage(),
            "fresh progress (level 0) must give the base damage of 1");
        Assert.AreEqual(Bullet.Owner.Player, _game.bullets[0].owner,
            "player bullets must belong to the player");
    }

    // ------------------------------------------------------------------
    // Asynchronous tests (one editor frame per step, like real gameplay)
    // ------------------------------------------------------------------

    [UnityTest]
    public IEnumerator RapidFire_ExpiresOverFrames()
    {
        Player player = CreatePlayer();
        player.ActivateRapidFire(2f, 0.3f);

        // 20 frames of 0.01 s = 0.2 s: still inside the 0.3 s duration.
        for (int frame = 0; frame < 20; frame++)
        {
            TickPowerupTimers(player, 0.01f);
            Assert.AreEqual(2f, player.GetFireRateMultiplier(),
                "multiplier must hold while the rapid fire timer is running");
            yield return null;
        }

        // 10 more frames pass the 0.3 s duration.
        for (int frame = 0; frame < 10; frame++)
        {
            TickPowerupTimers(player, 0.01f);
            yield return null;
        }

        Assert.AreEqual(1f, player.GetFireRateMultiplier(),
            "rapid fire must expire after its duration across frames");
    }

    [UnityTest]
    public IEnumerator Invincibility_RecoverThenTakeDamage_OverFrames()
    {
        Player player = CreatePlayer(health: 10);
        player.takeDamage(1);
        Assert.AreEqual(Player.PlayerState.Invulnerable, player.state);

        // Ten 0.1 s frames: i-frames end after exactly 1 second.
        for (int frame = 0; frame < 10; frame++)
        {
            player.CustomUpdate(0.1f);
            yield return null;
        }
        Assert.AreEqual(Player.PlayerState.Alive, player.state,
            "i-frames must end after 1 second across frames");

        player.takeDamage(1);
        Assert.AreEqual(8, player.health, "after recovery the next hit must apply again (10 -> 9 -> 8)");
        Assert.AreEqual(Player.PlayerState.Invulnerable, player.state,
            "the new hit must start fresh i-frames");
    }
}
