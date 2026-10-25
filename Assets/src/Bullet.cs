using UnityEngine;

public class Bullet : GameObjectBase
{
    public enum Owner { Player, Enemy }

    [Header("Config")]
    public Owner owner;
    public int damage = 1;
    public float lifetime = 10f;

    public Vector2 velocity;

    public override void CustomUpdate(float deltaTime)
    {
        position += velocity * deltaTime;                        // frame-rate independent
        lifetime -= deltaTime;
        if (lifetime <= 0f) Destroy(gameObject);
    }

    public int getDamage() => damage;
}