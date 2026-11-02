using UnityEngine;

public class TriangleEnemy : Enemy
{
    [Header("Movement")]
    public float moveSpeed = 100f; // units per second

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float fireInterval = 1f; // seconds between shots
    private float fireTimer;

    [HideInInspector] public Game game;

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
            GameObject newBulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Bullet newBullet = newBulletObj.GetComponent<Bullet>();
            if (newBullet != null)
            {
                game.bullets.Add(newBullet);
            }
        }
    }
}
