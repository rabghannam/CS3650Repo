using UnityEngine;

public class DecagonEnemy : Enemy
{
    [Header("Movement")]
    public float moveSpeed = 100f; // units per second

    private void Start()
    {
        SetMaxHealth(10); // Set health for Decagon enemy (10 sides)
    }

    public override void CustomUpdate(float deltaTime)
    {
        base.CustomUpdate(deltaTime); // Keep hit flash effect from Enemy

        // Move left continuously
        position += Vector2.left * moveSpeed * Time.deltaTime;
    }
}
