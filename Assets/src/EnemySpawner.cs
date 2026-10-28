using UnityEngine;

/// <summary>
/// Spawns enemies at random Y positions within a specified range.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs; // Assign all 8 enemy prefabs in inspector
    public float initialSpawnInterval = 5f; // starting seconds between spawns
    public float minSpawnInterval = 0.5f; // minimum seconds between spawns
    public float spawnIntervalDecreaseRate = 0.01f; // how fast interval decreases per second
    public float minY = -4f; // Minimum Y offset from transform
    public float maxY = 4f; // Maximum Y offset from transform

    [Header("References")]
    public Game game;

    private float spawnTimer;
    private float spawnInterval;
    private float elapsedTime;

    private void Awake()
    {
        if (game == null)
        {
            game = FindObjectOfType<Game>();
        }
        spawnInterval = initialSpawnInterval;
        spawnTimer = spawnInterval;
        elapsedTime = 0f;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        
        // Gradually decrease spawn interval over time, but don't go below minimum
        spawnInterval = Mathf.Max(initialSpawnInterval - (elapsedTime * spawnIntervalDecreaseRate), minSpawnInterval);
        
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
            // Randomly select one of the 8 enemy prefabs
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject selectedPrefab = enemyPrefabs[randomIndex];
            
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
