using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSpawner : MonoBehaviour
{
    public GameObject healthPrefab;
    public int minHealthPacks = 1;
    public int maxHealthPacks = 5;
    
    // Hardcoded size for internal logic
    private Vector3 healthPackSize = new Vector3(0.5f, 0.5f, 0.5f);

    [Tooltip("If true, spawns health automatically on Start. Uncheck for Round Mode.")]
    public bool spawnOnStart = true;

    // Start is called before the first frame update
    void Start()
    {
        if (spawnOnStart)
        {
            SpawnHealth(Random.Range(minHealthPacks, maxHealthPacks + 1));
        }
    }

    public void SpawnHealth(int count)
    {
        if (SpawnManager.Instance == null) return;

        Debug.Log($"HealthSpawner: Spawning {count} health packs.");

        for (int i = 0; i < count; i++)
        {
            SpawnSingleHealth();
        }
    }

    void SpawnSingleHealth()
    {
        if (healthPrefab == null)
        {
            Debug.LogError("HealthSpawner: Health Prefab is not assigned!");
            return;
        }

        if (SpawnManager.Instance.GetRandomPointOnFloor(healthPackSize, out Vector3 spawnPos, healthPackSize.y * 0.5f))
        {
            Instantiate(healthPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("HealthSpawner: Could not find valid spawn point.");
        }
    }
}
