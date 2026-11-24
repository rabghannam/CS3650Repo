# US-Boss: Elite Boss Enemy — Unit Testing (Red-Green-Refactor)

**Feature:** A new `Boss` enemy that appears exactly every 10 waves (10, 20, 30, ...),
has exactly 50 base health, a distinct elite visual treatment (larger, brighter),
and a special announcement on introduction.

**Approach:** TDD — the tests in `Assets/Tests/BossFeatureTests.cs` were written
**first** (Red), before any Boss/Wave code existed. Then the minimum code to make
them pass (Green): `Assets/src/Boss.cs` and `Assets/src/WaveManager.cs`.
Finally, the code was reviewed against SOLID principles (Refactor).

---

## Test assembly (single runner)

| File | Purpose |
|---|---|
| `Assets/Tests/Tests.asmdef` | The single test assembly for the whole project (references `CS3650Game`, `UnityEngine.TestRunner`, `UnityEditor.TestRunner`, `nunit.framework.dll`) |
| `Assets/src/CS3650Game.asmdef` | Groups all game code in one assembly so the test assembly can reference it (asmdefs cannot reference predefined `Assembly-CSharp`) |
| `Assets/Tests/BossFeatureTests.cs` | All unit tests for the Boss feature |

Open **Window → General → Test Runner → Edit Mode** to run every test with one runner.

## Implemented tests (8)

| # | Test | Requirement | Kind |
|---|---|---|---|
| 1 | `IsBossWave_TrueExactlyAtWaveMultiplesOfTen` | Boss every wave 10, 20, 30... (11 cases) | sync |
| 2 | `WavesBeforeFirstBossWave_NeverAnnounceOrSpawnBoss` | no boss before wave 10 | sync |
| 3 | `Boss_Initialized_HasExactlyFiftyBaseHealth` | exactly 50 base health | sync |
| 4 | `Boss_EliteVisuals_AreLargerAndBrighterThanRegularEnemy` | distinct + larger + brighter | sync |
| 5 | `BossWave_Starts_AnnouncesBossIntroductionExactlyOnce` | special announcement, once | sync |
| 6 | `Boss_DiesOnlyAfterFiftyDamage_SpreadAcrossFrames` | 50 HP frame-by-frame | **async** `[UnityTest]` |
| 7 | `BossAnnouncement_ShowsThenFadesOutOverFrames` | announcement then fade out | **async** `[UnityTest]` |
| 8 | `BossWave_SpawnsExactlyOneBossPerBossWave` | exactly one boss per boss wave (10 & 20) | **async** `[UnityTest]` |

Each test creates its own GameObjects in `[SetUp]`/`[TearDown]`, so any single unit
can be tested independently of the rest of the game (no scene required).

## What was implemented to make them pass (Green)

- `Assets/src/WaveManager.cs` — wave progression. `IsBossWave(wave)` = positive
  multiple of `BossWaveInterval` (10). On a boss wave it raises `BossWaveReached`,
  calls `AnnounceBoss()` (fires `BossAnnounced` with
  `BossAnnouncementText`, active for `AnnouncementDuration`) and spawns the
  `bossPrefab` if one is assigned (`BossSpawned`).
- `Assets/src/Boss.cs` — extends `Enemy` with `BaseHealth = 50` (`SetMaxHealth`),
  elite color gold `EliteColor`, elite scale `EliteScale = 2.5f`, and disables the
  regular green→red health gradient so the elite identity is preserved.
- `Assets/Prefabs/Boss.prefab` — now carries the `Boss` component,
  scale `2.5` and bright gold color in-editor (runtime elite visuals are enforced
  by `InitializeBoss()` anyway).
- `Assets/src/EnemySpawner.cs` — auto-adds a `WaveManager` beside itself in `Awake`
  (waves advance automatically in Play mode every `secondsPerWave`).
- `Assets/src/HUD.cs` — optional `announcementText` + `waveManager` fields; shows
  the boss warning and hides it after `announcementDuration`.

## Refactor notes (SOLID review)

- **SRP:** wave rules live in `WaveManager`; elite presentation lives in `Boss`.
  No wave logic inside enemy classes.
- **OCP:** `Boss` derives from `Enemy` — existing enemy/health/damage code was not
  modified, only extended. All old behavior is preserved.
- **LSP:** `Boss` is a valid `Enemy`; `CollisionSystem`, bullets and `Game.enemies`
  treat it like any other enemy.
- **ISP/DIP:** `WaveManager` communicates through C# events
  (`BossWaveReached`, `BossAnnounced`, `BossSpawned`) instead of referencing the
  HUD, so UI code depends on an event, not on concrete wave internals.
- **Testability:** wave cadence (`IsBossWave`) is a pure static function; all
  engine-dependent work is injected via prefab fields, keeping units independent.

## In the editor (manual wiring, 1 minute)

1. In `GameScene`, the `EnemySpawner` auto-creates its `WaveManager` at `Awake`.
   Optionally assign the WaveManager's **Boss Prefab** to `Prefabs/Boss`.
2. In the HUD canvas, assign a `Text` to `announcementText` and the `WaveManager`
   to `waveManager`, or leave them null (announcement then only fires events).
