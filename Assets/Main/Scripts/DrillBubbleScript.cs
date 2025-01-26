using UnityEngine;

public class DrillBubbleScript : MonoBehaviour
{
    // This function is called when the collider attached to this GameObject collides with another collider
    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("collision!!");

        //Debug.Log(collision.gameObject.name);
        // Check if the object's name starts with 'P'
            //Debug.Log("Starts with P!");
            // Get the HealthController component from the colliding object
        HealthController healthController = collision.gameObject.GetComponent<HealthController>();
        if (healthController != null)
        {
            //Debug.Log("healthController not null!");
            healthController.Heal(1);
            Destroy(gameObject);
        }

            // Destroy the colliding object (optional, based on your needs)
            //Destroy(collision.gameObject);
    }
}
