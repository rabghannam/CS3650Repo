using UnityEngine;

public class HexagonEnemy : Enemy
{
    [Header("Movement")]
    public float moveSpeed = 100f; // units per second

    private void Start()
    {
        SetMaxHealth(6); // Set health for Hexagon enemy (6 sides)
    }

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 200f;
    public float fireInterval = 1f; // seconds between shots
    private float fireTimer;

    public Game game;

    // 6 directions: right (0), up-right (45), up-left (135), left (180), down-left (225), down-right (315)
    private readonly float[] bulletAngles = { 0f, 45f, 135f, 180f, 225f, 315f };

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

        // Shooting logic - fire in 6 directions simultaneously
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
            // Enemy velocity vector (moving left)
            Vector3 enemyVelocity = Vector3.left * moveSpeed;
            
            foreach (float angle in bulletAngles)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                GameObject newBulletObj = Instantiate(bulletPrefab, transform.position, rotation);
                Bullet newBullet = newBulletObj.GetComponent<Bullet>();
                if (newBullet != null)
                {
                    // Set velocity relative to enemy's movement
                    // Bullet direction vector + enemy velocity vector
                    newBullet.velocity = newBulletObj.transform.right * bulletSpeed + enemyVelocity;
                    newBullet.owner = Bullet.Owner.Enemy;
                    game.bullets.Add(newBullet);
                }
            }
        }
    }
}
