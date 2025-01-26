using System.Collections;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject shadowObject; // The prefab you want to spawn
    public GameObject fallingObject; // The prefab to replace the spawned object

    public float waitTime = 5f;

    // Define your spawn zone boundaries
    public float minX, maxX;
    public float minY, maxY;
    public float minZ, maxZ;

    private void Start()
    {
        // Start the coroutine to spawn objects every 5 seconds
        StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            // Wait for 5 seconds
            yield return new WaitForSeconds(waitTime);

            // Generate a random position within the zone
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            float randomZ = Random.Range(minZ, maxZ);
            Vector3 spawnPosition = new Vector3(randomX, randomY, randomZ);

            // Spawn the object
            GameObject shadowObjectInstance = Instantiate(shadowObject, spawnPosition, Quaternion.identity);

            // Wait for 3 seconds before replacing the object
            yield return new WaitForSeconds(3f);

            // Replace the spawned object with the replacement object
            StartCoroutine(SpawnObjectFromShadow(shadowObjectInstance));
        }
    }

    private IEnumerator SpawnObjectFromShadow(GameObject shadowObjectInstance)
    {
        // Get the position and rotation of the old object
        Vector3 shadowPosition = shadowObjectInstance.transform.position;
        Quaternion rotation = shadowObjectInstance.transform.rotation;

        // Instantiate the replacement object 20 units above the original position
        Vector3 fallingObjectPosition = shadowPosition + new Vector3(0, 20f, 0);
        GameObject fallingObjectInstance = Instantiate(fallingObject, fallingObjectPosition, rotation);

        // Start the coroutine to move the replacement object down
        yield return StartCoroutine(MoveDown(fallingObjectInstance, shadowPosition, shadowObjectInstance));

        // Check if falling object has OnLanded to do
        TennisBallScript tennisBallScript = fallingObjectInstance.GetComponent<TennisBallScript>();
        if (tennisBallScript != null) {
            tennisBallScript.OnLanded();
        }
        
        // Enable Circle 2d collider
        CircleCollider2D circleCollider2D = fallingObjectInstance.GetComponent<CircleCollider2D>();
        circleCollider2D.enabled = true;
        
        if (!shadowObjectInstance)
        {
            yield return null;
        }
        
        // Start the coroutine to destroy the replacement object after 5 seconds
        yield return new WaitForSeconds(5f);
        Destroy(fallingObjectInstance);
    }

    private IEnumerator MoveDown(GameObject fallingObjectInstance, Vector3 targetPosition, GameObject shadowObjectInstance)
    {
        float speed = 20f; // Speed at which the object will move down
        float moveDuration = 20f / speed; // Time it will take to move down
        float timeToWaitBeforeDestroying = moveDuration - 1f; // Time to wait before destroying the old object

        // Wait for the specified time before destroying the old object
        yield return new WaitForSeconds(timeToWaitBeforeDestroying);
        
        Destroy(shadowObjectInstance);

        // Continue moving down to the target position
        while (fallingObjectInstance.transform.position.y > targetPosition.y)
        {
            fallingObjectInstance.transform.position = Vector3.MoveTowards(fallingObjectInstance.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }
}
