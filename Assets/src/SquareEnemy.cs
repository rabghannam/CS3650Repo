using UnityEngine;

public class SquareEnemy : Enemy
{
    [Header("Movement")]
    public float moveSpeed = 100f; // units per second

    [Header("Health")]
    public new int health = 4;

    public override void CustomUpdate(float deltaTime)
    {
        base.CustomUpdate(deltaTime); // Keep hit flash effect from Enemy

        // Move left continuously
        position += Vector2.left * moveSpeed * Time.deltaTime;
    }
}
