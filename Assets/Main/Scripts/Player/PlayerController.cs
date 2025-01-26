using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{

public float MaxSpeed;
public float acceleration;
public float steering;

public float drift = 2.0f;

Rigidbody2D rb;

float x;
float y = 1;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;
        
        //Debug.Log("collider: " + collider.gameObject.name);
        if(collider.gameObject.name.StartsWith("Drill"))
        {
            //Debug.Log("Drill: " + collider.gameObject.name);
            HealthController healthController = gameObject.GetComponent<HealthController>();
            if(healthController != null)
            {
                //Debug.Log("healthController: not null");
                healthController.Damage(3);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
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
        if (Input.GetJoystickNames().Length > 0 && x != 0 || y != 0)
        {
            // Handle your controller-specific logic here
            // Move the object based on controller input
            Move(x, y);
        //Debug.DrawLine(rb.position, rb.GetRelativePoint(relativeForce), Color.green, 2, false);
        }
    }

    protected void Move(float x, float y) {
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
    }
}
