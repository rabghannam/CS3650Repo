using UnityEngine;

/// <summary>
/// US-Boss: elite enemy that appears on boss waves.
/// - Exactly 50 base health (Boss.BaseHealth).
/// - Elite visual treatment: larger scale, brighter gold color, but same
///   hit-flash behaviour as any enemy (Open/Closed: only overrides visuals).
/// </summary>
public class Boss : Enemy
{
    /// <summary>Requirement: the boss must have exactly 50 base health.</summary>
    public const int BaseHealth = 50;

    /// <summary>Requirement: elite visuals are larger than a regular enemy.</summary>
    public const float EliteScale = 2.5f;

    /// <summary>Requirement: elite visuals are brighter than a regular enemy (gold vs green).</summary>
    public static readonly Color EliteColor = new Color(1f, 0.75f, 0.2f); // gold

    /// <summary>True once the elite visual treatment has been applied.</summary>
    public bool IsEliteActive { get; private set; }

    private void Start()
    {
        InitializeBoss();
    }

    /// <summary>
    /// Configures base health and elite visuals.
    /// Must be called in Start() or later (not Awake), matching SetMaxHealth contract.
    /// Public so unit tests can bring the boss into its configured state.
    /// </summary>
    public void InitializeBoss()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // Elite bosses keep a fixed gold color; stop the green->red health gradient
        // from repainting the elite color. Hit feedback (flash) still works.
        useHealthColorGradient = false;

        SetMaxHealth(BaseHealth);
        ApplyEliteVisuals();
    }

    /// <summary>Applies the distinct elite treatment: larger and brighter.</summary>
    public void ApplyEliteVisuals()
    {
        IsEliteActive = true;
        transform.localScale = Vector3.one * EliteScale;

        if (spriteRenderer != null)
        {
            originalColor = EliteColor;
            spriteRenderer.color = EliteColor;
        }
    }
}
