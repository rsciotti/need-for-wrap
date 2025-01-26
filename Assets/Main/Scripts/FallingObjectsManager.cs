using System.Collections;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // The prefab you want to spawn
    public GameObject replacementObject; // The prefab to replace the spawned object

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
            yield return new WaitForSeconds(5f);

            // Generate a random position within the zone
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            float randomZ = Random.Range(minZ, maxZ);
            Vector3 spawnPosition = new Vector3(randomX, randomY, randomZ);

            // Spawn the object
            GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);

            // Wait for 3 seconds before replacing the object
            yield return new WaitForSeconds(3f);

            // Replace the spawned object with the replacement object
            StartCoroutine(ReplaceObject(spawnedObject));
        }
    }

    private IEnumerator ReplaceObject(GameObject oldObject)
    {
        // Get the position and rotation of the old object
        Vector3 originalPosition = oldObject.transform.position;
        Quaternion rotation = oldObject.transform.rotation;

        // Instantiate the replacement object 20 units above the original position
        Vector3 replacementPosition = originalPosition + new Vector3(0, 100f, 0);
        GameObject newObject = Instantiate(replacementObject, replacementPosition, rotation);

        // Start the coroutine to move the replacement object down
        yield return StartCoroutine(MoveDown(newObject, originalPosition, oldObject));
    }

    private IEnumerator MoveDown(GameObject obj, Vector3 targetPosition, GameObject oldObject)
    {
        float speed = 50f; // Speed at which the object will move down
        float moveDuration = 20f / speed; // Time it will take to move down
        float timeToWaitBeforeDestroying = moveDuration - 0.01f; // Time to wait before destroying the old object

        // Wait for the specified time before destroying the old object
        yield return new WaitForSeconds(timeToWaitBeforeDestroying);
        Destroy(oldObject);

        // Continue moving down to the target position
        while (obj.transform.position.y > targetPosition.y)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }
}
