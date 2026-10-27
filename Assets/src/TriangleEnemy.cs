using UnityEngine;

public class TriangleEnemy : Enemy
{
    [Header("Movement")]
    public float moveSpeed = 100f; // units per second

    public override void CustomUpdate(float deltaTime)
    {
        base.CustomUpdate(deltaTime); // Keep hit flash effect from Enemy

        // Move left continuously
        position += Vector2.left * moveSpeed * Time.deltaTime;
    }
}
