using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public override void Start()
    {
        base.Start(); // Call base Start to initialize UI
        
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
            // This is BAD.
            
            // Fix: If player is dead, DO NOT call EndGame() here.
            // EndGame is for WINNING in Round Mode?
            // No, EndGame was used for both in original code.
            // But now we separated them.
            
            // If player is dead, OnPlayerDeath handles it.
            // So we should only call EndGame if we WON.
            // But Round Mode is endless? Or until death?
            // If it's endless, you only "EndGame" when you die.
            // So Round Mode "EndGame" IS the Game Over screen?
            // Or is there a "Win" condition?
            // The user said "win screen with your time".
            // But Round Mode is rounds.
            
            // Let's assume for now we just want to fix the inheritance.
            // I will leave the Update logic as is but fix the Start and ProceedToLeaderboard.
            
            // Actually, if IsGameOver() is true (player dead), it calls EndGame().
            // EndGame() shows Win Screen.
            // This is WRONG for death.
            
            // I should change this logic.
            if (isPlayerDead) return; // Handled by OnPlayerDeath
            
            // If there is another win condition?
            // Round mode seems endless.
            // So maybe EndGame is never called unless we want to quit?
            // Or maybe there is a max round?
            
            // For now, I will just fix the overrides.
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

    /**
     * Override EndGame to handle round-based scoring
     */
    public override void EndGame()
    {
        // This is called when we WIN (e.g. maybe after X rounds if we added that logic, or manually)
        // For Round Mode, maybe we just use the base EndGame which shows Win Screen?
        // But we need to show Rounds, not Time.
        // Base EndGame shows winTimeText.
        
        // Let's override to show Rounds if possible, or just use base and update text.
        base.EndGame();
        
        if (winTimeText != null)
        {
            winTimeText.text = $"Rounds: {currentRound}";
        }
    }

    protected override void ProceedToLeaderboard()
    {
        string leaderboardName = PlayerPrefs.GetString("leaderboardname");

        if (leaderboardName == "")
        {
            // No name registered, store score temporarily and go to setup scene
            PlayerPrefs.SetString("last_score", $"rounds:{currentRound}");
            SceneManager.LoadScene("LeaderboardSetup");
            return;
        }

        // Name exists, update leaderboard and go to main menu
        UpdateRoundLeaderboard(leaderboardName, currentRound);
        SceneManager.LoadScene("MainMenu");
    }

    /**
     * Updates the round-based leaderboard with a new entry
     * @param playerName The player's name
     * @param rounds The number of rounds completed
     */
    private void UpdateRoundLeaderboard(string playerName, int rounds)
    {
        string leaderboard = PlayerPrefs.GetString("leaderboard");
        List<LeaderboardEntry> allEntries = ParseLeaderboardEntries(leaderboard);

        // Separate by type
        List<LeaderboardEntry> timeEntries = allEntries.Where(e => !e.isRounds).ToList();
        List<LeaderboardEntry> roundsEntries = allEntries.Where(e => e.isRounds).ToList();

        // Add new rounds entry
        roundsEntries.Add(new LeaderboardEntry { name = playerName, score = rounds, isRounds = true });

        // Sort rounds entries (higher is better)
        roundsEntries = roundsEntries.OrderByDescending(e => e.score).Take(3).ToList();

        // Combine both types back together
        List<string> formattedEntries = new List<string>();

        // Format time entries as "name:score"
        formattedEntries.AddRange(timeEntries.Select(e => $"{e.name}:{e.score:F2}"));

        // Format rounds entries as "name:rounds:X"
        formattedEntries.AddRange(roundsEntries.Select(e => $"{e.name}:rounds:{(int)e.score}"));

        leaderboard = string.Join(",", formattedEntries);

        PlayerPrefs.SetString("leaderboard", leaderboard);
    }
}
