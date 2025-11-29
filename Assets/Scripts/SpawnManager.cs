using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    
    [Tooltip("Layers to check for collisions when spawning.")]
    public LayerMask collisionLayerMask; 

    private GameObject[] floorObjects;

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            RefreshFloorObjects(); // Initialize immediately
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // RefreshFloorObjects(); // Already done in Awake
    }

    public void RefreshFloorObjects()
    {
        floorObjects = GameObject.FindGameObjectsWithTag("Floor");
        if (floorObjects.Length == 0)
        {
            Debug.LogError("SpawnManager: No objects with tag 'Floor' found! Please tag your floor objects.");
        }
        else
        {
            Debug.Log($"SpawnManager: Found {floorObjects.Length} floor objects.");
        }
    }

    /// <summary>
    /// Tries to find a valid spawn point on a random floor object.
    /// </summary>
    /// <param name="sizeToCheck">The size of the box to check for collisions (full size, not half extents).</param>
    /// <param name="result">The valid spawn position.</param>
    /// <param name="yOffset">Height offset from the floor.</param>
    /// <param name="attempts">Number of attempts to find a spot.</param>
    /// <param name="edgeMargin">Distance from the edge of the floor to avoid.</param>
    /// <returns>True if a valid point was found.</returns>
    public bool GetRandomPointOnFloor(Vector3 sizeToCheck, out Vector3 result, float yOffset = 0f, int attempts = 10, float edgeMargin = 0f)
    {
        result = Vector3.zero;
        if (floorObjects == null || floorObjects.Length == 0) return false;

        for (int i = 0; i < attempts; i++)
        {
            GameObject randomFloor = floorObjects[Random.Range(0, floorObjects.Length)];
            Collider floorCol = randomFloor.GetComponent<Collider>();
            
            if (floorCol == null) continue;

            Vector3 randomPoint = GetRandomPointInBounds(floorCol.bounds, edgeMargin);
            randomPoint.y += yOffset; // Adjust for object height (e.g. half height of object)
            
            // Lift the check box slightly so it doesn't intersect the floor itself
            Vector3 checkPos = randomPoint + (Vector3.up * 0.1f);

            // CheckBox uses halfExtents
            if (!Physics.CheckBox(checkPos, sizeToCheck / 2, Quaternion.identity, collisionLayerMask))
            {
                result = randomPoint;
                return true;
            }
        }

        return false;
    }

    Vector3 GetRandomPointInBounds(Bounds bounds, float margin)
    {
        float minX = bounds.min.x + margin;
        float maxX = bounds.max.x - margin;
        float minZ = bounds.min.z + margin;
        float maxZ = bounds.max.z - margin;

        // Ensure we don't cross over if margin is too big
        if (minX > maxX) { float temp = minX; minX = maxX; maxX = temp; }
        if (minZ > maxZ) { float temp = minZ; minZ = maxZ; maxZ = temp; }

        return new Vector3(
            Random.Range(minX, maxX),
            bounds.max.y,
            Random.Range(minZ, maxZ)
        );
    }
}
