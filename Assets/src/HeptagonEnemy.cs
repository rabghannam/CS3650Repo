using UnityEngine;

public class HeptagonEnemy : Enemy
{
    [Header("Movement")]
    public float moveSpeed = 100f; // units per second

    [Header("Health")]
    public new int health = 7;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 200f;
    public float fireInterval = 1f; // seconds between shots
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

        // Shooting logic - fire to the left
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
            Quaternion rotation = Quaternion.Euler(0, 0, 180f); // Point left
            GameObject newBulletObj = Instantiate(bulletPrefab, transform.position, rotation);
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
