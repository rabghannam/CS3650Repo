using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("Health bar — US-011")]
    public Image healthBarFill;
    public Text healthText;

    [Header("Red flash — US-009")]
    public Image flashOverlay;                 // full-screen Image, stretch to canvas
    public const float FLASH_DURATION = 1f;    // exactly 1 second, matches i-frames
    public float flashMaxAlpha = 0.45f;         // noticeable but non-blinding

    private float flashTimer;

    private void Start()
    {
        if (flashOverlay != null) SetFlashAlpha(0f);
    }

    private void Update()
    {
        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;                         // frame-rate independent
            float t = Mathf.Clamp01(flashTimer / FLASH_DURATION); // fade out over 1 s
            SetFlashAlpha(flashMaxAlpha * t);
            if (flashTimer <= 0f) SetFlashAlpha(0f);
        }
    }

    /// <summary>US-011: real-time health bar + current/max values + color gradient.</summary>
    public void updateHealth(int current, int max)
    {
        float ratio = Mathf.Clamp01((float)current / max);

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = ratio;
            healthBarFill.color = getHealthColor(ratio);
        }
        if (healthText != null)
            healthText.text = current + " / " + max;
    }

    /// <summary>Gradient: green (100%) -> yellow (50%) -> red (25% and below).</summary>
    public Color getHealthColor(float ratio)
    {
        if (ratio > 0.75f) return Color.Lerp(new Color(1f, 0.92f, 0.23f), Color.green,  // yellow -> green
                                            Mathf.InverseLerp(0.75f, 1f, ratio));
        if (ratio > 0.5f) return Color.Lerp(new Color(1f, 0.25f, 0.1f),               // red -> yellow
                                            new Color(1f, 0.92f, 0.23f),
                                            Mathf.InverseLerp(0.5f, 0.75f, ratio));
        return new Color(1f, 0.25f, 0.1f);                                             // red at <= 25%
    }

    /// <summary>US-009: screen flashes red for exactly 1 second.</summary>
    public void triggerRedFlash()
    {
        flashTimer = FLASH_DURATION;
        SetFlashAlpha(flashMaxAlpha);
    }

    private void SetFlashAlpha(float a)
    {
        if (flashOverlay == null) return;
        Color c = flashOverlay.color;
        c.r = 1f; c.g = 0f; c.b = 0f;
        c.a = a;
        flashOverlay.color = c;
        flashOverlay.raycastTarget = false;
    }
}