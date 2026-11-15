using UnityEngine;

public class HomingBullet : Bullet
{
    [Header("Homing")]
    public float speed = 200f;
    public float turnSpeed = 180f;
    private Transform playerTransform;

    private void Awake()
    {
        // Find the Player object and store its transform
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    public override void CustomUpdate(float deltaTime)
    {
        if (playerTransform != null)
        {
            // Calculate direction to target
            Vector2 targetDirection = (Vector2)(playerTransform.position - transform.position);
            
            // Get angle to target
            float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            
            // Current rotation in degrees
            float currentAngle = transform.eulerAngles.z;
            
            // Calculate angle difference and normalize to [-180, 180]
            float angleDiff = (targetAngle - currentAngle + 540f) % 360f - 180f;
            
            // Calculate max rotation this frame based on turn speed
            float maxRotation = turnSpeed * deltaTime;
            
            // Rotate towards target (clamped to max rotation)
            float newAngle = currentAngle + Mathf.Clamp(angleDiff, -maxRotation, maxRotation);
            
            // Apply rotation
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
            
            // Move forward in current direction
            position += (Vector2)transform.right * speed * deltaTime;
            velocity = (Vector2)transform.right * speed;
        }
        else
        {
            // Fallback: move with current velocity if no target
            position += velocity * deltaTime;
        }

        lifetime -= deltaTime;
        if (lifetime <= 0f) Destroy(gameObject);
    }
}
