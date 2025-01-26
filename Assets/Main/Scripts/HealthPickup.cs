using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // This function is called when the collider attached to this GameObject collides with another collider
    void OnTriggerEnter2D(Collider2D collision)
    {
        HealthController healthController = collision.gameObject.GetComponent<HealthController>();
        if (healthController != null)
        {
            //Debug.Log("healthController not null!");
            healthController.Heal(3);
            Destroy(gameObject);
        }

    }
}
