using UnityEngine;
using UnityEngine.Events;

public class Player2Controller : MonoBehaviour
{

public float MaxSpeed;
public float acceleration;
public float steering;

public float drift = 2.0f;

Rigidbody2D rb;

float x;
float y = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

    }
    void FixedUpdate()
    {
        Vector2 speed = transform.up * (y * acceleration);
        rb.AddForce(speed);

        float direction = Vector2.Dot(rb.linearVelocity, rb.GetRelativeVector(Vector2.up));

        if(acceleration > 0)
        {
            if(direction > 0)
            {
                rb.rotation -= x * steering * (rb.linearVelocity.magnitude / MaxSpeed);
            }
            else
            {
                rb.rotation += x * steering * (rb.linearVelocity.magnitude / MaxSpeed);
            }
        }

        float driftForce = Vector2.Dot(rb.linearVelocity, rb.GetRelativeVector(Vector2.left)) * drift;

        Vector2 relativeForce = Vector2.right * driftForce;

        rb.AddForce(rb.GetRelativeVector(relativeForce));

        if(rb.linearVelocity.magnitude > MaxSpeed)
        {
            rb.linearVelocity = rb. linearVelocity.normalized * MaxSpeed;
        }

        //Debug.DrawLine(rb.position, rb.GetRelativePoint(relativeForce), Color.green, 2, false);
    }
}
