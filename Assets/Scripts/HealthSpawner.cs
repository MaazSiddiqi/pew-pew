using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSpawner : MonoBehaviour
{
    public GameObject healthPrefab;
    public int minHealthPacks = 1;
    public int maxHealthPacks = 5;
    public float minDistance = 2f;

    private List<GameObject> activeHealthPacks = new List<GameObject>();
    private GameObject[] floorObjects;

    // Start is called before the first frame update
    void Start()
    {
        floorObjects = GameObject.FindGameObjectsWithTag("Floor");
        if (floorObjects.Length == 0)
        {
            Debug.LogError("No objects with tag 'Floor' found! Please tag your floor objects.");
        }
        else
        {
            SpawnRandomHealth();
        }
    }

    void SpawnRandomHealth()
    {
        int count = Random.Range(minHealthPacks, maxHealthPacks + 1);
        Debug.Log($"Spawning {count} health packs.");

        for (int i = 0; i < count; i++)
        {
            SpawnHealth();
        }
    }

    void SpawnHealth()
    {
        if (floorObjects.Length == 0 || healthPrefab == null) return;

        // 1. Pick a random floor object
        GameObject randomFloor = floorObjects[Random.Range(0, floorObjects.Length)];
        Collider floorCol = randomFloor.GetComponent<Collider>();

        if (floorCol == null) return;

        // 2. Pick a random point within bounds
        Vector3 randomPoint = GetRandomPointInBounds(floorCol.bounds);

        // 3. Check distance to other packs
        bool tooClose = false;
        foreach (GameObject pack in activeHealthPacks)
        {
            if (pack != null && Vector3.Distance(randomPoint, pack.transform.position) < minDistance)
            {
                tooClose = true;
                break;
            }
        }

        if (!tooClose)
        {
            // Spawn slightly above floor to avoid clipping
            Vector3 spawnPos = randomPoint + Vector3.up * 0.5f;
            GameObject newPack = Instantiate(healthPrefab, spawnPos, Quaternion.identity);
            activeHealthPacks.Add(newPack);
        }
    }

    Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.max.y, // Spawn on top surface
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
