using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public int minEnemies = 3;
    public int maxEnemies = 8;
    
    // Hardcoded size for internal logic
    private Vector3 enemySize = new Vector3(1f, 2f, 1f);

    [Tooltip("If true, spawns enemies automatically on Start. Uncheck for Round Mode.")]
    public bool spawnOnStart = true;

    void Start()
    {
        Debug.Log("EnemySpawner: Script has started.");
        if (spawnOnStart)
        {
            SpawnEnemies(Random.Range(minEnemies, maxEnemies + 1));
        }
    }

    public void SpawnEnemies(int count)
    {
        if (SpawnManager.Instance == null)
        {
            Debug.LogError("EnemySpawner: SpawnManager instance not found!");
            return;
        }

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("EnemySpawner: No Enemy Prefabs assigned in Inspector!");
            return;
        }

        Debug.Log($"EnemySpawner: Spawning {count} enemies.");

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            
            // Try to find a valid point
            // Offset y by half height so it sits on floor
            if (SpawnManager.Instance.GetRandomPointOnFloor(enemySize, out Vector3 spawnPos, enemySize.y * 0.5f))
            {
                Instantiate(prefab, spawnPos, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("EnemySpawner: Could not find a valid spawn point for enemy.");
            }
        }
    }
}
