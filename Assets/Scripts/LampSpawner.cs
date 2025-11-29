using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampSpawner : MonoBehaviour
{
    public GameObject lampPrefab;
    public int minLamps = 5;
    public int maxLamps = 10;
    
    // Hardcoded size for internal logic
    private Vector3 lampSize = new Vector3(0.5f, 2f, 0.5f);

    void Start()
    {
        Debug.Log("LampSpawner: Script has started.");
        SpawnLamps();
    }

    void SpawnLamps()
    {
        if (SpawnManager.Instance == null) return;

        if (lampPrefab == null)
        {
            Debug.LogError("LampSpawner: Lamp Prefab is not assigned in Inspector!");
            return;
        }

        int count = Random.Range(minLamps, maxLamps + 1);
        Debug.Log($"LampSpawner: Spawning {count} lamps.");

        for (int i = 0; i < count; i++)
        {
            if (SpawnManager.Instance.GetRandomPointOnFloor(lampSize, out Vector3 spawnPos, lampSize.y * 0.5f))
            {
                Instantiate(lampPrefab, spawnPos, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("LampSpawner: Could not find valid spawn point.");
            }
        }
    }
}
