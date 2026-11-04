using UnityEngine;

/// <summary>
/// Money item that can be collected by the player.
/// Drops from enemies with 80% probability.
/// </summary>
public class Money : GameObjectBase
{
    [Header("Value")]
    public int amount = 10;
    
    [Header("Movement")]
    public float moveSpeed = 50f;
    
    [Header("Lifetime")]
    public float lifetime = 10f;
    
    private float timer;
    
    public override void CustomUpdate(float deltaTime)
    {
        // Money moves downward slowly
        position += Vector2.down * moveSpeed * deltaTime;
        
        // Count down lifetime
        timer += deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // When player collects money
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            // Add money to player's score
            // Note: This assumes the HUD or Game class handles scoring
            Destroy(gameObject);
        }
    }
}
