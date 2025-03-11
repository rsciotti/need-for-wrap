using UnityEngine;

public class InverterPickup : MonoBehaviour
{
    // This function is called when the collider attached to this GameObject collides with another collider
    void OnTriggerEnter2D(Collider2D collision)
    {
        BaseVehicle baseVehicle = collision.GetComponent<BaseVehicle>();
        if (baseVehicle != null)
        {
            baseVehicle.inverseStart();
            Destroy(gameObject);
        }
    }
}
