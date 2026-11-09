using UnityEngine;

public class TriangleEnemy : Enemy
{
    [Header("Movement")]
    public float moveSpeed = 100f; // units per second

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 200f;
    [Range(0f, 180f)]
    public float bulletSpreadAngle = 30f; // total spread angle between outermost bullets
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

        // Shooting logic - fire to the left every second
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
            // Spawn 3 bullets with configurable spread (centered at 180° = left)
            float halfSpread = bulletSpreadAngle * 0.5f;
            float[] angles = { 180f - halfSpread, 180f, 180f + halfSpread };
            foreach (float angle in angles)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                GameObject newBulletObj = Instantiate(bulletPrefab, transform.position, rotation);
                Bullet newBullet = newBulletObj.GetComponent<Bullet>();
                if (newBullet != null)
                {
                    // Set velocity based on bullet's rotation (move in local forward direction)
                    newBullet.velocity = newBulletObj.transform.right * bulletSpeed;
                    newBullet.owner = Bullet.Owner.Enemy;
                    game.bullets.Add(newBullet);
                }
            }
        }
    }
}
