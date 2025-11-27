using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSpawner : MonoBehaviour
{
    public GameObject ammoPrefab;
    public int minAmmoBoxes = 3;
    public int maxAmmoBoxes = 10;
    public float minDistance = 2f;

    private List<GameObject> activeAmmoBoxes = new List<GameObject>();
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
            SpawnRandomAmmo();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // No respawning logic needed
    }

    void SpawnRandomAmmo()
    {
        int count = Random.Range(minAmmoBoxes, maxAmmoBoxes + 1);
        Debug.Log($"Spawning {count} ammo boxes.");

        for (int i = 0; i < count; i++)
        {
            SpawnAmmo();
        }
    }

    void SpawnAmmo()
    {
        if (floorObjects.Length == 0 || ammoPrefab == null) return;

        // 1. Pick a random floor object
        GameObject randomFloor = floorObjects[Random.Range(0, floorObjects.Length)];
        Collider floorCol = randomFloor.GetComponent<Collider>();

        if (floorCol == null) return;

        // 2. Pick a random point within bounds
        Vector3 randomPoint = GetRandomPointInBounds(floorCol.bounds);

        // 3. Check distance to other boxes
        bool tooClose = false;
        foreach (GameObject box in activeAmmoBoxes)
        {
            if (box != null && Vector3.Distance(randomPoint, box.transform.position) < minDistance)
            {
                tooClose = true;
                break;
            }
        }

        if (!tooClose)
        {
            // Spawn slightly above floor to avoid clipping
            Vector3 spawnPos = randomPoint + Vector3.up * 0.5f;
            GameObject newBox = Instantiate(ammoPrefab, spawnPos, Quaternion.identity);
            activeAmmoBoxes.Add(newBox);
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
