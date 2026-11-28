using UnityEngine;

/// <summary>
/// Temporal Echo System recorder (GDD 4.3).
/// Hold Shift to record the player's movement (max 3.0 s), press E to deploy
/// an Echo that replays the stored path. One recording is stored; a new one
/// overwrites the old. Deployment starts the cooldown (15.0 s base).
/// </summary>
public class EchoRecorder : MonoBehaviour
{
    public const float MAX_RECORD_DURATION = 3f;   // GDD: 3.0 sec (upgradable to 6.0)
    public const float DEPLOY_COOLDOWN = 15f;      // GDD: 15.0 sec (upgradable to 5.0)

    [Header("Wiring (set by Game)")]
    public Player player;
    public Game game;
    public GameObject bulletPrefab;
    public float damageMultiplier = 1f;            // GDD: 1.0x (upgradable to 2.0x)

    private EchoRecording _stored;                 // one storage slot
    private EchoRecording _current;                // recording in progress
    private float _recordTime;
    private float _cooldownRemaining;
    private bool _recordWasHeld;

    public bool IsRecording { get; private set; }
    public bool HasStoredRecording => _stored != null;
    public EchoRecording StoredRecording => _stored;
    public bool CanDeploy => _cooldownRemaining <= 0f;
    public float CooldownRemaining => _cooldownRemaining;

    /// <summary>
    /// Advances recording and cooldown by deltaTime (frame-rate independent;
    /// all timing is measured in seconds, not frames).
    /// </summary>
    public void Tick(float deltaTime, Vector2 playerPosition, bool recordHeld)
    {
        if (_cooldownRemaining > 0f)
            _cooldownRemaining = Mathf.Max(0f, _cooldownRemaining - deltaTime);

        if (!IsRecording && recordHeld && !_recordWasHeld)
            StartRecording();                      // rising edge of Shift

        if (IsRecording)
        {
            if (!recordHeld)
            {
                StopRecording();
            }
            else
            {
                _recordTime = Mathf.Min(MAX_RECORD_DURATION, _recordTime + deltaTime);
                _current.AddSample(_recordTime, playerPosition);
                if (_recordTime >= MAX_RECORD_DURATION)
                    StopRecording();               // auto-stop at the 3.0 s cap
            }
        }

        _recordWasHeld = recordHeld;
    }

    private void StartRecording()
    {
        _current = new EchoRecording();
        _recordTime = 0f;
        IsRecording = true;
    }

    private void StopRecording()
    {
        IsRecording = false;
        if (_current != null && _current.SampleCount > 0)
            _stored = _current;                    // one slot: overwrites the old recording
        _current = null;
    }

    /// <summary>Spawns an Echo that replays the stored recording (GDD: press E).</summary>
    public Echo Deploy()
    {
        if (!HasStoredRecording || !CanDeploy) return null;

        Echo echo = new GameObject("Echo").AddComponent<Echo>();
        echo.recording = _stored;
        echo.player = player;
        echo.game = game;
        echo.bulletPrefab = bulletPrefab;
        echo.damageMultiplier = damageMultiplier;
        echo.position = _stored.Evaluate(0f);      // start where the recording started

        _cooldownRemaining = DEPLOY_COOLDOWN;
        return echo;
    }

    private void Update()
    {
        if (player == null) return;                // tests drive Tick() directly

        bool recordHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        Tick(Time.deltaTime, player.position, recordHeld);

        if (Input.GetKeyDown(KeyCode.E) && CanDeploy)
            Deploy();
    }
}
