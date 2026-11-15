using UnityEngine;

public class NonagonEnemy : Enemy
{
    [Header("Movement")]
    public float moveSpeed = 100f; // units per second

    private void Start()
    {
        SetMaxHealth(9); // Set health for Nonagon enemy (9 sides)
    }

    public override void CustomUpdate(float deltaTime)
    {
        base.CustomUpdate(deltaTime); // Keep hit flash effect from Enemy

        // Move left continuously
        position += Vector2.left * moveSpeed * Time.deltaTime;
    }
}
