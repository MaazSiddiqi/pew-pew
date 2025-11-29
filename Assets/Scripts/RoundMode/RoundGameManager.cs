using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundGameManager : GameManager
{
    [Header("Round Settings")]
    public int currentRound = 0;
    public float roundCountdown = 10f;
    public bool isRoundActive = false;

    [Header("References")]
    public GameObject spawnerController;
    public RoundPlayerUI roundPlayerUI;

    private EnemySpawner enemySpawner;
    private AmmoSpawner ammoSpawner;
    private HealthSpawner healthSpawner;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Find references
        if (roundPlayerUI == null) roundPlayerUI = FindObjectOfType<RoundPlayerUI>();

        if (spawnerController != null)
        {
            enemySpawner = spawnerController.GetComponent<EnemySpawner>();
            ammoSpawner = spawnerController.GetComponent<AmmoSpawner>();
            healthSpawner = spawnerController.GetComponent<HealthSpawner>();
        }
        else
        {
            // Fallback if user forgot to assign controller
            enemySpawner = FindObjectOfType<EnemySpawner>();
            ammoSpawner = FindObjectOfType<AmmoSpawner>();
            healthSpawner = FindObjectOfType<HealthSpawner>();
        }

        StartCountdown();
    }

    void Update()
    {
        if(IsGameOver()){
            EndGame();
            return;
        }

        if (!isRoundActive)
        {
            roundCountdown -= Time.deltaTime;
            if (roundPlayerUI != null) roundPlayerUI.UpdateRoundCountdown(roundCountdown);

            if (roundCountdown <= 0)
            {
                StartRound();
            }
        }
        
        timeElapsed += Time.deltaTime;
    }

    void StartCountdown()
    {
        isRoundActive = false;
        roundCountdown = 10f;
        if (roundPlayerUI != null) roundPlayerUI.UpdateRoundCountdown(roundCountdown);
    }

    void StartRound()
    {
        currentRound++;
        isRoundActive = true;
        
        // Update UI
        if (roundPlayerUI != null)
        {
            roundPlayerUI.UpdateRoundText(currentRound);
            roundPlayerUI.UpdateRoundCountdown(0); // Hide countdown
        }

        // Calculate spawns
        int enemiesToSpawn = currentRound;
        int itemsToSpawn = Mathf.CeilToInt(3 + (currentRound * 1.5f));

        // Spawn Enemies
        if (enemySpawner != null)
        {
            enemySpawner.SpawnEnemies(enemiesToSpawn);
        }

        // Spawn Items
        if (ammoSpawner != null) ammoSpawner.SpawnAmmo(itemsToSpawn);
        if (healthSpawner != null) healthSpawner.SpawnHealth(itemsToSpawn);
    }

    public override void OnEnemySpawned(){
        base.OnEnemySpawned();
        if (roundPlayerUI != null) roundPlayerUI.UpdateEnemyCountText(enemyCount);
    }

    public override void OnEnemyDeath(){
        base.OnEnemyDeath();
        if (roundPlayerUI != null) roundPlayerUI.UpdateEnemyCountText(enemyCount);

        if(enemyCount <= 0 && isRoundActive){
            EndRound();
        }
    }

    void EndRound()
    {
        Debug.Log($"Round {currentRound} Complete!");
        StartCountdown();
    }

    public override bool IsGameOver()
    {
        return isPlayerDead;
    }
}
