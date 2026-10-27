using UnityEngine;

/// <summary>
/// Spawns enemies at random Y positions within a specified range.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs; // Array of all 8 enemy types
    public float spawnInterval = 2f; // seconds between spawns
    public float minY = -4f; // Minimum Y offset from transform
    public float maxY = 4f; // Maximum Y offset from transform

    [Header("References")]
    public Game game;

    private float spawnTimer;

    private void Awake()
    {
        if (game == null)
        {
            game = FindObjectOfType<Game>();
        }
        spawnTimer = spawnInterval;
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            GameObject selectedPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            float randomY = Random.Range(minY, maxY);
            Vector2 spawnPosition = new Vector2(transform.position.x, transform.position.y + randomY);
            GameObject newEnemyObj = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
            
            if (game != null)
            {
                Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();
                if (newEnemy != null)
                {
                    game.enemies.Add(newEnemy);
                }
            }
        }
    }
}
