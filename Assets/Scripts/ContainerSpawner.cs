using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerSpawner : MonoBehaviour
{
    public GameObject[] containerPrefabs;
    public int minContainers = 2;
    public int maxContainers = 6;
    
    // Hardcoded size for internal logic
    private Vector3 containerSize = new Vector3(2f, 2f, 2f);

    void Start()
    {
        Debug.Log("ContainerSpawner: Script has started.");
        SpawnContainers();
    }

    void SpawnContainers()
    {
        if (SpawnManager.Instance == null) return;

        if (containerPrefabs == null || containerPrefabs.Length == 0)
        {
            Debug.LogError("ContainerSpawner: No Container Prefabs assigned in Inspector!");
            return;
        }

        int count = Random.Range(minContainers, maxContainers + 1);
        Debug.Log($"ContainerSpawner: Spawning {count} containers.");

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = containerPrefabs[Random.Range(0, containerPrefabs.Length)];
            
            // Use 2f margin to keep away from edges
            if (SpawnManager.Instance.GetRandomPointOnFloor(containerSize, out Vector3 spawnPos, containerSize.y * 0.5f, 10, 2f))
            {
                Instantiate(prefab, spawnPos, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("ContainerSpawner: Could not find valid spawn point.");
            }
        }
    }
}
