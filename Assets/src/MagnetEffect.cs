using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the magnet powerup effect that attracts collectible items to the player.
/// Attaches to the player and pulls nearby Money and Powerup objects.
/// </summary>
public class MagnetEffect : MonoBehaviour
{
    [Header("References")]
    public Player player;
    
    [Header("Magnet Settings")]
    public float magnetRange = 200f;
    public float magnetSpeed = 300f;
    
    private bool isActive = false;
    private float timer = 0f;
    
    // Lists to track collectible items
    private List<Money> moneyList = new List<Money>();
    private List<Powerup> powerupList = new List<Powerup>();
    
    private void Start()
    {
        if (player == null)
        {
            player = GetComponent<Player>();
        }
    }
    
    private void Update()
    {
        // Check if magnet is active from player
        if (player != null && player.HasMagnet())
        {
            player.GetMagnetParams(out magnetRange, out magnetSpeed);
            isActive = true;
        }
        else
        {
            isActive = false;
        }
        
        if (isActive)
        {
            // Update money list
            moneyList.RemoveAll(m => m == null);
            Money[] moneyInScene = FindObjectsOfType<Money>();
            foreach (Money m in moneyInScene)
            {
                if (!moneyList.Contains(m))
                {
                    moneyList.Add(m);
                }
            }
            
            // Update powerup list
            powerupList.RemoveAll(p => p == null);
            Powerup[] powerupsInScene = FindObjectsOfType<Powerup>();
            foreach (Powerup p in powerupsInScene)
            {
                if (!powerupList.Contains(p))
                {
                    powerupList.Add(p);
                }
            }
            
            // Attract all collectibles within range
            AttractItems();
        }
    }
    
    private void AttractItems()
    {
        Vector2 playerPos = transform.position;
        
        // Attract money
        foreach (Money money in moneyList)
        {
            if (money != null && money.gameObject != null)
            {
                float distance = Vector2.Distance(playerPos, money.position);
                if (distance <= magnetRange)
                {
                    // Move money towards player
                    Vector2 direction = (playerPos - money.position).normalized;
                    money.position += direction * magnetSpeed * Time.deltaTime;
                    
                    // Slow down as it gets closer
                    if (distance < 50f)
                    {
                        money.position += direction * magnetSpeed * 2f * Time.deltaTime;
                    }
                }
            }
        }
        
        // Attract powerups
        foreach (Powerup powerup in powerupList)
        {
            if (powerup != null && powerup.gameObject != null)
            {
                float distance = Vector2.Distance(playerPos, powerup.position);
                if (distance <= magnetRange)
                {
                    // Move powerup towards player
                    Vector2 direction = (playerPos - powerup.position).normalized;
                    powerup.position += direction * magnetSpeed * Time.deltaTime;
                    
                    // Slow down as it gets closer
                    if (distance < 50f)
                    {
                        powerup.position += direction * magnetSpeed * 2f * Time.deltaTime;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Activates the magnet effect with specified parameters
    /// </summary>
    public void Activate(float range, float speed, float duration)
    {
        magnetRange = range;
        magnetSpeed = speed;
        isActive = true;
        timer = duration;
        
        // Start coroutine to deactivate after duration
        StartCoroutine(DeactivateAfterDelay(duration));
    }
    
    private System.Collections.IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isActive = false;
    }
}
