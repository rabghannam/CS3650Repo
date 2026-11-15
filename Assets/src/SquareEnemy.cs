using UnityEngine;

public class SquareEnemy : Enemy
{
    [Header("Movement")]
    public float moveSpeed = 100f; // units per second

    [Header("Health")]
    public new int health = 4;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 200f;
    public float fireInterval = 1f; // seconds between shots
    public float bulletYSpacing = 0.5f; // vertical spacing between bullets
    private float fireTimer;

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

        // Shooting logic - fire to the left every fireInterval seconds
        if (bulletPrefab != null)
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                SpawnBullets();
                fireTimer = fireInterval;
            }
        }
    }

    private void SpawnBullets()
    {
        if (bulletPrefab != null && game != null)
        {
            // Spawn 5 bullets pointing left (180 degrees) with vertical offset
            int numBullets = 5;
            float startY = -bulletYSpacing * 2f; // Center the 5 bullets around enemy's y position
            
            for (int i = 0; i < numBullets; i++)
            {
                float yOffset = startY + bulletYSpacing * i;
                Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
                Quaternion rotation = Quaternion.Euler(0, 0, 180f); // Point left
                
                GameObject newBulletObj = Instantiate(bulletPrefab, spawnPosition, rotation);
                Bullet newBullet = newBulletObj.GetComponent<Bullet>();
                if (newBullet != null)
                {
                    // Set velocity to move left
                    newBullet.velocity = Vector2.left * bulletSpeed;
                    newBullet.owner = Bullet.Owner.Enemy;
                    game.bullets.Add(newBullet);
                }
            }
        }
    }
}
