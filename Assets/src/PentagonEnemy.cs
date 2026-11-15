using UnityEngine;

public class PentagonEnemy : Enemy
{
    [Header("Movement")]
    public float moveSpeed = 100f; // units per second

    private void Start()
    {
        SetMaxHealth(5); // Set health for Pentagon enemy (5 sides)
    }

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 200f;
    public float fireInterval = 0.3f; // seconds between shots
    [Range(0f, 180f)]
    public float angleIncrement = 15f; // angle increase per shot
    private float fireTimer;
    private float currentAngle = 180f; // start at 180 degrees (left)

    public Game game;

    private void Awake()
    {
        if (game == null)
        {
            game = FindObjectOfType<Game>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
        lifetimeTimer = lifetime;
    }

    public override void CustomUpdate(float deltaTime)
    {
        base.CustomUpdate(deltaTime); // Keep hit flash effect from Enemy

        // Move left continuously
        position += Vector2.left * moveSpeed * Time.deltaTime;

        // Shooting logic - fire spiral pattern
        if (bulletPrefab != null)
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                SpawnBullet();
                fireTimer = fireInterval;
            }
        }
    }

    private void SpawnBullet()
    {
        if (bulletPrefab != null && game != null)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            GameObject newBulletObj = Instantiate(bulletPrefab, transform.position, rotation);
            Bullet newBullet = newBulletObj.GetComponent<Bullet>();
            if (newBullet != null)
            {
                // Enemy velocity vector (moving left)
                Vector3 enemyVelocity = Vector3.left * moveSpeed;
                
                // Set velocity relative to enemy's movement
                // Bullet direction vector + enemy velocity vector
                newBullet.velocity = newBulletObj.transform.right * bulletSpeed + enemyVelocity;
                newBullet.owner = Bullet.Owner.Enemy;
                game.bullets.Add(newBullet);
            }
            
            // Increment angle for next shot to create spiral pattern
            currentAngle += angleIncrement;
            // Normalize angle to stay within 0-360 range
            if (currentAngle >= 360f)
                currentAngle -= 360f;
            if (currentAngle < 0f)
                currentAngle += 360f;
        }
    }
}
