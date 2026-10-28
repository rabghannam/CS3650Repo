using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBounce : MonoBehaviour
{
    [SerializeField]
    private LayerMask bounceLayerMask;
    [SerializeField]
    private float bounceForce = 5.0f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if ((bounceLayerMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null && collision.contacts.Length > 0)
            {
                Vector2 normal = collision.contacts[0].normal;
                Vector2 reflectedVelocity = Vector2.Reflect(rb.velocity.normalized, normal);
                rb.velocity = reflectedVelocity * bounceForce;
            }
        }
    }
}