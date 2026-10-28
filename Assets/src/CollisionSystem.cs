using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central collision detection. "Pixel-perfect" hits are approximated with
/// accurate 2D overlap tests (tight Box/Circle Collider2D). If true
/// sprite-alpha collision is required, swap checkPixelPerfect for a
/// texture-read implementation.
/// </summary>
public class CollisionSystem : MonoBehaviour
{
    /// <summary>Overlap test used for "pixel-perfect" hit registration.</summary>
    public bool checkPixelPerfect(Collider2D a, Collider2D b)
    {
        if (a == null || b == null) return false;
        return a.IsTouching(b);
    }

    public bool bulletHitsEnemy(Bullet bullet, Enemy enemy) =>
        bullet.owner == Bullet.Owner.Player && checkPixelPerfect(bullet.GetComponent<Collider2D>(), enemy.GetComponent<Collider2D>());

    public bool bulletHitsPlayer(Bullet bullet, Player player) =>
        bullet.owner == Bullet.Owner.Enemy && checkPixelPerfect(bullet.GetComponent<Collider2D>(), player.GetComponent<Collider2D>());

    public bool playerHitsEnemy(Player player, Enemy enemy) =>
        checkPixelPerfect(player.GetComponent<Collider2D>(), enemy.GetComponent<Collider2D>());

    /// <summary>Called by Game each frame: resolves all bullet-entity collisions.</summary>
    public void resolveCollisions(List<Bullet> bullets, List<Enemy> enemies, Player player)
    {
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            Bullet b = bullets[i];

            if (b.owner == Bullet.Owner.Player)
            {
                foreach (Enemy e in enemies)
                {
                    if (e != null && e.isAlive )
                    {
                        if (bulletHitsEnemy(b, e))
                        {
                            e.takeDamage(b.getDamage());             // US-005: effect + health reduction
                            Destroy(b.gameObject);
                            bullets.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        // Player takes damage on direct collision with enemy
        if (player != null && player.isAlive())
        {
            foreach (Enemy e in enemies)
            {
                if (e != null && e.isAlive && playerHitsEnemy(player, e))
                {
                    player.takeDamage(1);
                }
            }
        }
    }
}