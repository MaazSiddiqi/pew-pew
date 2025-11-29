using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSpawner : MonoBehaviour
{
    public GameObject ammoPrefab;
    public int minAmmoBoxes = 3;
    public int maxAmmoBoxes = 10;
    
    // Hardcoded size for internal logic
    private Vector3 ammoBoxSize = new Vector3(0.5f, 0.5f, 0.5f);

    [Tooltip("If true, spawns ammo automatically on Start. Uncheck for Round Mode.")]
    public bool spawnOnStart = true;

    // Start is called before the first frame update
    void Start()
    {
        if (spawnOnStart)
        {
            SpawnAmmo(Random.Range(minAmmoBoxes, maxAmmoBoxes + 1));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // No respawning logic needed
    }

    public void SpawnAmmo(int count)
    {
        if (SpawnManager.Instance == null) return;

        Debug.Log($"AmmoSpawner: Spawning {count} ammo boxes.");

        for (int i = 0; i < count; i++)
        {
            SpawnSingleAmmo();
        }
    }

    void SpawnSingleAmmo()
    {
        if (ammoPrefab == null)
        {
            Debug.LogError("AmmoSpawner: Ammo Prefab is not assigned in the Inspector!");
            return;
        }

        if (SpawnManager.Instance.GetRandomPointOnFloor(ammoBoxSize, out Vector3 spawnPos, ammoBoxSize.y * 0.5f))
        {
            Instantiate(ammoPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"AmmoSpawner: Spawned ammo at {spawnPos}");
        }
        else
        {
            Debug.LogWarning("AmmoSpawner: Could not find valid spawn point.");
        }
    }
}
