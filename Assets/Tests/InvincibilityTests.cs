/**
 *  Tests for the invicibility frames
 *  Author: David Sanchez
*/
/**
 *  Tests for the features implemented in this conversation.
 *  Task 1 - US-009: 1-second invincibility frames after taking damage.
 *
 *  Requirements under test (US-009):
 *    1. 1-second invincibility after taking damage (frame-rate independent).
 *    2. Extra hits during the invincibility period do nothing (no death chains).
 *    3. Visual indicator (red flash) shows the active invincibility period.
 *    4. Player can move and fire during invincibility.
 *    5. Invincibility can be toggled OFF in settings; default is ON.
 *
 *  All tests for this conversation live in this single file so the whole
 *  feature set runs from one Test Runner class.
 */
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class InvincibilityTests
{
    private Player _player;
    private HUD _hud;
    private Game _game;
    private SettingsManager _settings;
    private SettingsPanel _settingsPanel;
    private readonly List<GameObject> _spawned = new List<GameObject>();

    private const string INVINCIBILITY_KEY = "Settings_Invincibility";

    [SetUp]
    public void SetUp()
    {
        // Start each test from a clean persisted-settings state (default ON).
        PlayerPrefs.DeleteKey(INVINCIBILITY_KEY);
        PlayerPrefs.DeleteKey("Progress_Money");
        PlayerPrefs.DeleteKey("Progress_MaxHealthLevel");
        PlayerPrefs.DeleteKey("Progress_FireRateLevel");
        PlayerPrefs.DeleteKey("Progress_BaseDamageLevel");
    }

    [TearDown]
    public void TearDown()
    {
        // Bullets spawned by the player during tests (must go before _game is destroyed).
        if (_game != null)
            foreach (Bullet b in _game.bullets)
                if (b != null) Object.DestroyImmediate(b.gameObject);

        if (_player != null) Object.DestroyImmediate(_player.gameObject);
        if (_hud != null) Object.DestroyImmediate(_hud.gameObject);
        if (_game != null) Object.DestroyImmediate(_game.gameObject);
        if (_settings != null) Object.DestroyImmediate(_settings.gameObject);
        if (_settingsPanel != null) Object.DestroyImmediate(_settingsPanel.gameObject);

        foreach (GameObject obj in _spawned)
            if (obj != null) Object.DestroyImmediate(obj);
        _spawned.Clear();

        // Remove auto-created singletons so tests stay independent.
        ProgressManager leftoverProgress = Object.FindObjectOfType<ProgressManager>();
        if (leftoverProgress != null) Object.DestroyImmediate(leftoverProgress.gameObject);
        SettingsManager leftoverSettings = Object.FindObjectOfType<SettingsManager>();
        if (leftoverSettings != null) Object.DestroyImmediate(leftoverSettings.gameObject);

        PlayerPrefs.DeleteKey(INVINCIBILITY_KEY);
        PlayerPrefs.DeleteKey("Progress_Money");
        PlayerPrefs.DeleteKey("Progress_MaxHealthLevel");
        PlayerPrefs.DeleteKey("Progress_FireRateLevel");
        PlayerPrefs.DeleteKey("Progress_BaseDamageLevel");
        PlayerPrefs.Save();
    }

    // ------------------------------------------------------------------
    // Helpers
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
        _player.input = obj.AddComponent<InputHandler>();
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
        return obj;
    }

    /// <summary>SettingsManager created fresh (field default ON, no saved prefs).</summary>
    private SettingsManager CreateSettings()
    {
        GameObject obj = new GameObject("TestSettingsManager");
        _spawned.Add(obj);
        _settings = obj.AddComponent<SettingsManager>();
        return _settings;
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

    // ------------------------------------------------------------------
    // Requirement 1: 1-second invincibility after taking damage
    // ------------------------------------------------------------------

    [Test]
    public void TakeDamage_EntersOneSecondInvincibility_AppliesDamageOnce()
    {
        Player player = CreatePlayer();

        player.takeDamage(1);

        Assert.AreEqual(Player.BASE_HEALTH - 1, player.health, "Damage must be applied exactly once");
        Assert.AreEqual(Player.PlayerState.Invulnerable, player.state, "Player must enter Invulnerable after damage");
        Assert.AreEqual(Player.IFRAME_DURATION, player.iFrameTimer, "I-frame timer must start at exactly 1 second");
        Assert.AreEqual(1f, Player.IFRAME_DURATION, "IFRAME_DURATION constant must be 1 second");
    }

    [Test]
    public void Invincibility_LastsExactlyOneSecond_FrameRateIndependent()
    {
        Player player = CreatePlayer();
        player.takeDamage(1);

        // Simulate 0.9 s in many small frames (high frame rate): still invincible.
        for (int i = 0; i < 9; i++)
            player.CustomUpdate(0.1f);
        Assert.AreEqual(Player.PlayerState.Invulnerable, player.state,
            "Player must still be invincible just before 1 second has elapsed");

        // The final 0.1 s: i-frames must end exactly at 1 s.
        player.CustomUpdate(0.1f);
        Assert.AreEqual(Player.PlayerState.Alive, player.state,
            "I-frames must end after exactly 1 second");
        Assert.AreEqual(0f, player.iFrameTimer, "I-frame timer must clamp to 0 when it expires");
    }

    [Test]
    public void Invincibility_EndsAtOneSecond_RegardlessOfFrameSize()
    {
        // Few large frames (low frame rate) must also end i-frames at 1 s.
        Player player = CreatePlayer();
        player.takeDamage(1);

        player.CustomUpdate(0.7f);
        Assert.AreEqual(Player.PlayerState.Invulnerable, player.state,
            "I-frames must survive a 0.7 s frame");

        player.CustomUpdate(0.7f);
        Assert.AreEqual(Player.PlayerState.Alive, player.state,
            "I-frames must end once the 1 s total has elapsed (0.7 + 0.7 >= 1)");
    }

    // ------------------------------------------------------------------
    // Requirement 1b: extra hits during i-frames do nothing (no death chains)
    // ------------------------------------------------------------------

    [Test]
    public void TakeDamage_DuringInvincibility_IsIgnored()
    {
        Player player = CreatePlayer();
        player.takeDamage(1);
        int healthAfterFirstHit = player.health;

        player.takeDamage(1);
        player.takeDamage(1);

        Assert.AreEqual(healthAfterFirstHit, player.health,
            "Hits during invincibility must not reduce health");
        Assert.AreEqual(Player.PlayerState.Invulnerable, player.state,
            "Hits during invincibility must not reset or change the state");
        Assert.AreEqual(Player.IFRAME_DURATION, player.iFrameTimer,
            "Hits during invincibility must not extend the i-frame timer");
    }

    [Test]
    public void LowHealth_RepeatedHitsWithinIframes_CannotKillInstantly()
    {
        Player player = CreatePlayer(health: 2);

        player.takeDamage(1); // 2 -> 1, i-frames start
        player.takeDamage(1); // ignored (i-frames)
        player.takeDamage(1); // ignored (i-frames)

        Assert.AreEqual(1, player.health, "Death chain within 1 second must be prevented by i-frames");
        Assert.IsTrue(player.isAlive(), "Player must survive hits absorbed by i-frames");
    }

    [Test]
    public void InvincibilityExpires_PlayerTakesDamageAgain()
    {
        Player player = CreatePlayer();
        player.takeDamage(1);
        player.CustomUpdate(Player.IFRAME_DURATION + 0.05f);
        Assert.AreEqual(Player.PlayerState.Alive, player.state, "I-frames must have expired");

        player.takeDamage(1);
        Assert.AreEqual(Player.BASE_HEALTH - 2, player.health,
            "Damage after i-frames expire must be applied");
        Assert.AreEqual(Player.PlayerState.Invulnerable, player.state,
            "A fresh hit after i-frames must grant a fresh invincibility period");
    }

    // ------------------------------------------------------------------
    // Requirement 2: visual indicator shows active invincibility period
    // ------------------------------------------------------------------

    [Test]
    public void TakeDamage_TriggersRedFlashVisualIndicator()
    {
        Player player = CreatePlayer();

        player.takeDamage(1);

        Color flash = _hud.flashOverlay.color;
        Assert.AreEqual(1f, flash.r, "Flash overlay must be red");
        Assert.AreEqual(0f, flash.g, "Flash overlay must be red");
        Assert.AreEqual(0f, flash.b, "Flash overlay must be red");
        Assert.AreEqual(_hud.flashMaxAlpha, flash.a,
            "Flash overlay must be visible while invincibility is active");
    }

    [Test]
    public void RedFlashDuration_MatchesIframeDuration()
    {
        Assert.AreEqual(Player.IFRAME_DURATION, HUD.FLASH_DURATION,
            "Red flash indicator must stay visible for exactly the invincibility period");
    }

    // ------------------------------------------------------------------
    // Requirement 3: player can move during invincibility
    // ------------------------------------------------------------------

    [Test]
    public void Player_CanMoveDuringInvincibility()
    {
        Player player = CreatePlayer();
        ScriptedInput scripted = player.gameObject.AddComponent<ScriptedInput>();
        scripted.direction = new Vector2(1f, 0f); // move right
        player.input = scripted;

        player.takeDamage(1); // enters Invulnerable
        float startX = player.position.x;

        float dt = Time.deltaTime;
        player.CustomUpdate(dt);

        Assert.AreEqual(Player.PlayerState.Invulnerable, player.state,
            "Player must still be invincible while moving");
        Assert.Greater(player.position.x, startX,
            "Player must move during invincibility");

        float expectedDx = scripted.direction.normalized.x * player.moveSpeed * dt;
        Assert.AreEqual(expectedDx, player.position.x - startX, 0.01f,
            "Movement speed during invincibility must match normal move speed");
    }

    [Test]
    public void Player_MovesSameSpeedDuringInvincibilityAsWhenAlive()
    {
        ScriptedInput scripted = null;

        Player invulnerablePlayer = CreatePlayer();
        scripted = invulnerablePlayer.gameObject.AddComponent<ScriptedInput>();
        scripted.direction = new Vector2(1f, 0f);
        invulnerablePlayer.input = scripted;
        invulnerablePlayer.takeDamage(1); // Invulnerable

        Player alivePlayer = CreatePlayer();
        scripted = alivePlayer.gameObject.AddComponent<ScriptedInput>();
        scripted.direction = new Vector2(1f, 0f);
        alivePlayer.input = scripted;

        float dt = Time.deltaTime;
        invulnerablePlayer.CustomUpdate(dt);
        alivePlayer.CustomUpdate(dt);

        float invulnerableDx = invulnerablePlayer.position.x;
        float aliveDx = alivePlayer.position.x;
        Assert.AreEqual(aliveDx, invulnerableDx, 0.01f,
            "Movement during invincibility must be identical to movement while alive");
    }

    // ------------------------------------------------------------------
    // Requirement 3b: player can fire during invincibility
    // ------------------------------------------------------------------

    [Test]
    public void Player_CanFireDuringInvincibility()
    {
        Player player = CreatePlayer();
        player.bulletPrefab = CreateBulletPrefab();
        player.fireMode = Player.FireMode.Automatic;

        player.takeDamage(1); // enters Invulnerable
        Assert.AreEqual(Player.PlayerState.Invulnerable, player.state);

        player.CustomUpdate(Time.deltaTime);

        Assert.AreEqual(1, _game.bullets.Count,
            "Player must be able to fire while invincible");
        Assert.AreEqual(Player.PlayerState.Invulnerable, player.state,
            "Firing must not cancel the invincibility period");
        Assert.Greater(Player.IFRAME_DURATION - player.iFrameTimer, 0f,
            "I-frame timer must keep counting down while firing");
    }

    // ------------------------------------------------------------------
    // Requirement 4: invincibility toggle in settings, default ON
    // ------------------------------------------------------------------

    [Test]
    public void InvincibilitySetting_DefaultsToOn()
    {
        SettingsManager settings = CreateSettings();
        settings.LoadSettings(); // no saved prefs: must fall back to default

        Assert.IsTrue(settings.InvincibilityEnabled,
            "Invincibility must default to ON when no saved setting exists");
    }

    [Test]
    public void InvincibilitySetting_ToggledOff_PersistsAcrossSaveAndLoad()
    {
        SettingsManager settings = CreateSettings();
        settings.InvincibilityEnabled = false;
        settings.SaveSettings();

        // A brand new manager (as after a game restart) must load the saved OFF value.
        if (_settings != null) Object.DestroyImmediate(_settings.gameObject);
        SettingsManager reloaded = CreateSettings();
        reloaded.LoadSettings();

        Assert.IsFalse(reloaded.InvincibilityEnabled,
            "Toggled OFF invincibility must persist across save/load");
    }

    [Test]
    public void SettingsPanel_ToggleOff_UpdatesInvincibilitySetting()
    {
        CreateSettings();

        GameObject obj = new GameObject("TestSettingsPanel");
        _spawned.Add(obj);
        _settingsPanel = obj.AddComponent<SettingsPanel>();

        // The panel handler is private UI wiring; invoke it the way the Toggle would.
        MethodInfo handler = typeof(SettingsPanel).GetMethod("OnInvincibilityChanged",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(handler, "SettingsPanel must wire an invincibility toggle handler");

        handler.Invoke(_settingsPanel, new object[] { false });
        Assert.IsFalse(SettingsManager.Instance.InvincibilityEnabled,
            "Turning the toggle OFF must disable invincibility in settings");

        handler.Invoke(_settingsPanel, new object[] { true });
        Assert.IsTrue(SettingsManager.Instance.InvincibilityEnabled,
            "Turning the toggle ON must enable invincibility in settings");
    }

    // ------------------------------------------------------------------
    // Requirement 4b: player respects the settings toggle
    // ------------------------------------------------------------------

    [Test]
    public void InvincibilityDisabled_PlayerTakesDamageEveryHit_NoIframes()
    {
        Player player = CreatePlayer();
        SettingsManager settings = CreateSettings();
        settings.InvincibilityEnabled = false; // toggled OFF in settings

        player.takeDamage(1);
        player.takeDamage(1);

        Assert.AreEqual(Player.BASE_HEALTH - 2, player.health,
            "With invincibility OFF every hit must apply (user opted out of protection)");
        Assert.AreEqual(Player.PlayerState.Alive, player.state,
            "With invincibility OFF the player must never enter the Invulnerable state");
        Assert.AreEqual(0f, player.iFrameTimer, "No i-frame timer may run while invincibility is OFF");
    }

    [Test]
    public void InvincibilityReEnabled_IframesWorkAgain()
    {
        Player player = CreatePlayer();
        SettingsManager settings = CreateSettings();

        settings.InvincibilityEnabled = false;
        player.takeDamage(1);
        Assert.AreEqual(Player.PlayerState.Alive, player.state, "I-frames OFF: no invincibility");

        settings.InvincibilityEnabled = true; // toggle back ON
        player.takeDamage(1);
        Assert.AreEqual(Player.PlayerState.Invulnerable, player.state,
            "Re-enabling the setting must restore i-frames on the next hit");
        Assert.AreEqual(Player.IFRAME_DURATION, player.iFrameTimer,
            "Restored i-frames must last the full 1 second");
    }
}
