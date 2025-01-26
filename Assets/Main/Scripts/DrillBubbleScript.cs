using UnityEngine;

public class DrillBubbleScript : MonoBehaviour
{
    public GameObject drillPrefab; // Reference to the drill prefab

    // This function is called when the collider attached to this GameObject collides with another collider
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("collision!!");

        // Debug.Log(collision.gameObject.name);
        // Get the HealthController component from the colliding object
        HealthController healthController = collision.gameObject.GetComponent<HealthController>();
        if (healthController != null)
        {
            // Debug.Log("healthController not null!");
            healthController.Heal(1);

            // Append the drill to the front of the colliding object
            AttachDrill(collision.gameObject);

            // Destroy the current game object
            Destroy(gameObject);
        }
    }

    private void AttachDrill(GameObject target)
    {
        // Calculate the front position of the target object
        //Vector3 frontPosition = target.transform.position + target.transform.up * (target.GetComponent<Collider2D>().bounds.extents.y * 1.5 + drillPrefab.GetComponent<Collider2D>().bounds.extents.y);
        Vector3 frontPosition = target.transform.position + target.transform.up * (target.GetComponent<SpriteRenderer>().sprite.bounds.size.y * 0.3f + drillPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.y);
        // Instantiate the drill at the front position and parent it to the target object
        GameObject drillInstance = Instantiate(drillPrefab, frontPosition, target.transform.rotation);
        drillInstance.transform.SetParent(target.transform);
    }
}
