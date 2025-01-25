using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float drag = 0.1f;
    [SerializeField] private float turnSpeed = 200f;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Vector2 velocity;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void PlayerInput()
    {
        movement += playerControls.Movement.Move.ReadValue<Vector2>();
    }

    private void Move()
    {
        // Calculate the desired velocity based on input
        Vector2 desiredVelocity = movement * moveSpeed;

        // Interpolate between current velocity and desired velocity to create momentum
        velocity = Vector2.Lerp(velocity, desiredVelocity, Time.fixedDeltaTime);

        // Apply drag to simulate friction
        velocity *= (1 - drag * Time.fixedDeltaTime);

        // Move the object based on velocity
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

        // Calculate rotation based on the velocity direction, making sure it's smooth
        if (velocity != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.LerpAngle(rb.rotation, targetAngle, turnSpeed * Time.fixedDeltaTime);
            rb.rotation = angle;
        }
    }
}
