using Main.Scripts;
using Unity.Mathematics.Geometry;
using UnityEngine;

/// <summary>
/// Share common functionality between Player and AI
/// </summary>
public abstract class BaseVehicle : MonoBehaviour
{
    public float MaxSpeed;
    public float acceleration;
    public float steering;

    public float drift = 2.0f;

    public float rayDistance = 1.0f;
    public LayerMask collisionLayer;
    public int frontCollisionDamage = 1;

    protected Rigidbody2D rb;
    protected Collider2D collider2D;
    private float topAngle;

    protected Material baseMaterial;
    protected Material currentMaterial;
    protected bool inverseState = false;
    protected int inverseFrames = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();

        Vector2 size = GetComponent<BoxCollider2D>().size;
        size = Vector2.Scale (size, (Vector2)transform.localScale);
        topAngle = Mathf.Atan(size.x / size.y) * Mathf.Rad2Deg;

        baseMaterial = GetComponent<SpriteRenderer>().material;
        currentMaterial = baseMaterial;
    }

    protected abstract void FixedUpdate();

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleNormalFrontCollision(collision);
        HandleDrillCollision(collision);
    }

    private void HandleNormalFrontCollision(Collision2D collision) {
        Vector3 v = (Vector3)collision.contacts[0].point - transform.position;

        if (Vector3.Angle(v, transform.up) <= topAngle) {
            //Debug.Log("Front collision with: " + collision.gameObject.name);
            HealthController healthController = collision.gameObject.GetComponent<HealthController>();
            if (healthController != null) {
                healthController.Damage(frontCollisionDamage);
            }
        }
    }

    private void HandleDrillCollision(Collision2D collision) {
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

    protected void Move(float x, float y) {
        Vector2 speed = transform.up * (y * acceleration);
        rb.AddForce(speed);

        if(acceleration > 0)
        { 
            rb.rotation -= x * steering * Mathf.Clamp((rb.linearVelocity.magnitude / MaxSpeed), .5f, 1f);
        }

        float driftForce = Vector2.Dot(rb.linearVelocity, rb.GetRelativeVector(Vector2.left)) * drift;

        Vector2 relativeForce = Vector2.right * driftForce;

        rb.AddForce(rb.GetRelativeVector(relativeForce));

        if(rb.linearVelocity.magnitude > MaxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * MaxSpeed;
        }
    }

    public bool GetInverseState()
    {
        return inverseState;
    }

    public void inverseStart()
    {
        inverseState = true;
        inverseFrames = 500;
        currentMaterial = new Material(GameManager.Instance.GetComponent<SpriteRenderer>().material.shader);
        currentMaterial.mainTexture = baseMaterial.mainTexture;
        GetComponent<SpriteRenderer>().material = currentMaterial;
    }

    protected void inverseTick()
    {
        inverseFrames -= 1;
        if (inverseFrames <= 0)
        {
            inverseState = false;
            currentMaterial = baseMaterial;
            GetComponent<SpriteRenderer>().material = currentMaterial;
        }
    }
}
