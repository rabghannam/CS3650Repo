using UnityEngine;

public class OctagonEnemy : Enemy
{
    [Header("Movement")]
    public float moveSpeed = 100f; // units per second

    [Header("Health")]
    public new int health = 8;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 200f;
    public float fireInterval = 1f; // seconds between shots
    public float verticalOffset = 0.5f; // vertical distance between rows
    public float horizontalSpacing = 0.4f; // horizontal distance between bullets in each row
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

        // Shooting logic - fire 8 bullets in 2 rows of 4
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
            Quaternion rotation = Quaternion.Euler(0, 0, 180f); // Point left
            
            // Spawn 4 bullets in top row
            float[] xOffsets = { -1.5f, -0.5f, 0.5f, 1.5f };
            
            foreach (float xOffset in xOffsets)
            {
                // Top row
                Vector3 topPosition = new Vector3(
                    transform.position.x + xOffset * horizontalSpacing,
                    transform.position.y + verticalOffset,
                    transform.position.z
                );
                GameObject newBulletObj = Instantiate(bulletPrefab, topPosition, rotation);
                Bullet newBullet = newBulletObj.GetComponent<Bullet>();
                if (newBullet != null)
                {
                    newBullet.velocity = Vector2.left * bulletSpeed;
                    newBullet.owner = Bullet.Owner.Enemy;
                    game.bullets.Add(newBullet);
                }
                
                // Bottom row
                Vector3 bottomPosition = new Vector3(
                    transform.position.x + xOffset * horizontalSpacing,
                    transform.position.y - verticalOffset,
                    transform.position.z
                );
                newBulletObj = Instantiate(bulletPrefab, bottomPosition, rotation);
                newBullet = newBulletObj.GetComponent<Bullet>();
                if (newBullet != null)
                {
                    newBullet.velocity = Vector2.left * bulletSpeed;
                    newBullet.owner = Bullet.Owner.Enemy;
                    game.bullets.Add(newBullet);
                }
            }
        }
    }
}
