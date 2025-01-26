using UnityEngine;
using Main.Scripts;

public class TennisBallScript : MonoBehaviour
{
    // This function is called when the collider attached to this GameObject collides with another collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision!!");

        //Debug.Log(collision.gameObject.name);
        // Check if the object's name starts with 'P'
            //Debug.Log("Starts with P!");
            // Get the HealthController component from the colliding object
        HealthController healthController = collision.gameObject.GetComponent<HealthController>();
        if (healthController != null)
        {
                //Debug.Log("healthController not null!");
                // Call the Damage function and pass in the integer value of 5
            healthController.Damage(5);
        }

            // Destroy the colliding object (optional, based on your needs)
            //Destroy(collision.gameObject);
    }

    public void OnLanded() {
        var bounds = GetComponent<Collider2D>().bounds;
        GameManager.Instance.GetWrapController().PopWithinBounds(bounds.min, bounds.max);
    }
}
