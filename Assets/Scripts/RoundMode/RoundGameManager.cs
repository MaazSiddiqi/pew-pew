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

        // Hide Classic Mode UI
        if (timeElapsedText != null) timeElapsedText.gameObject.SetActive(false);
        if (promptText != null) promptText.gameObject.SetActive(false);
        if (winZone != null) winZone.SetActive(false);
    }

    void Update()
    {
        // Check for Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (isPaused) return;

        if(IsGameOver()){
            GameOver();
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

    /**
     * Override EndGame to handle round-based scoring
     */
    public override void EndGame()
    {
        // Use base EndGame to show the Win Screen (which we use as End Screen for Round Mode)
        base.EndGame();
        
        if (winTimeText != null)
        {
            winTimeText.text = $"Rounds Survived: {currentRound}";
        }
    }

    public override void GameOver()
    {
        // In Round Mode, Game Over (Death/Fall) leads to the same "End Screen" as winning
        // So we call EndGame() instead of the base GameOver()
        EndGame();
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
