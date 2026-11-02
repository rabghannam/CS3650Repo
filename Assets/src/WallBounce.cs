using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBounce : MonoBehaviour
{
    [SerializeField]
    private LayerMask bounceLayerMask;
    [SerializeField]
    private float bounceForce = 5.0f;
    [SerializeField]
    private float bounceDuration = 0.2f;

    private InputHandler inputHandler;
    private Vector2 bounceDirection;
    private float bounceSpeed;
    private float bounceTimer = 0f;
    private const float bounceDamping = 0.8f;

    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
    }

    void Update()
    {
        if (bounceTimer > 0)
        {
            transform.Translate(bounceDirection * bounceSpeed * Time.deltaTime, Space.World);
            bounceTimer -= Time.deltaTime;

            if (bounceTimer <= 0 && inputHandler != null)
            {
                inputHandler.enabled = true;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if ((bounceLayerMask.value & (1 << collision.gameObject.layer)) != 0 && inputHandler != null)
        {
            if (collision.contacts.Length > 0)
            {
                Vector2 normal = collision.contacts[0].normal;
                InputHandler.InputState state = inputHandler.readKeyboard();
                
                if (state.moveDirection.magnitude > 0.1f)
                {
                    bounceDirection = Vector2.Reflect(state.moveDirection, normal).normalized;
                    bounceSpeed = bounceForce * bounceDamping;
                    inputHandler.enabled = false;
                    bounceTimer = bounceDuration;
                }
            }
        }
    }
}