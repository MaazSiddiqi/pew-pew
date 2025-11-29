using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public TextMeshProUGUI timeElapsedText;
    public TextMeshProUGUI promptText;
    public GameObject winZone;

    [Header("UI Screens")]
    public GameObject gameOverScreen;
    public GameObject gameWinScreen;
    public GameObject pauseMenuScreen;
    public TextMeshProUGUI winTimeText;
    public PlayerUI playerUI;

    public int enemyCount = 0;
    public bool isPlayerDead = false;
    public bool isPaused = false;

    public float timeElapsed = 0f;

    public string gameMode = "Classic";
    public bool areEnemiesSpawned = false;

    public virtual void Start(){
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (PlayerPrefs.HasKey("GameMode"))
        {
            gameMode = PlayerPrefs.GetString("GameMode");
        }
        Debug.Log("Game Mode: " + gameMode);

        // Initialize UI
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (gameWinScreen != null) gameWinScreen.SetActive(false);
        if (pauseMenuScreen != null) pauseMenuScreen.SetActive(false);
        
        // Ensure time scale is reset
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;

        if (playerUI == null) playerUI = FindObjectOfType<PlayerUI>();
        if (playerUI != null) playerUI.ToggleCrosshair(true);
    }

    void Update(){
        // Check for Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (isPaused) return;

        timeElapsed += Time.deltaTime;
        timeElapsedText.text = timeElapsed.ToString("F2");

        if (enemyCount <= 0 && areEnemiesSpawned)
        {
            winZone.SetActive(true);
            promptText.gameObject.SetActive(true);
            promptText.text = "Enemies cleared! Head over to the end zone to finish!";
            promptText.color = Color.green;
        } else {
            winZone.SetActive(false);
        }
    }

    public void TogglePause()
    {
        // Don't allow pause if game is over or won
        if (isPlayerDead || (enemyCount <= 0 && areEnemiesSpawned && winZone.activeSelf)) return;

        isPaused = !isPaused;

        if (pauseMenuScreen != null)
        {
            pauseMenuScreen.SetActive(isPaused);
        }

        if (isPaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (playerUI != null) playerUI.ToggleCrosshair(false);
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (playerUI != null) playerUI.ToggleCrosshair(true);
        }
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }

    public virtual void OnEnemySpawned(){
        enemyCount++;
        areEnemiesSpawned = true;
    }

    /**
    * Ends the game (Win Condition)
    */
    public virtual void EndGame(){
        // Show Win Screen
        if (gameWinScreen != null)
        {
            gameWinScreen.SetActive(true);
            
            // Update time text if available
            if (winTimeText != null)
            {
                winTimeText.text = "Time: " + timeElapsed.ToString("F2") + "s";
            }
            
            // Pause game and unlock cursor
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (playerUI != null) playerUI.ToggleCrosshair(false);
        }
        else
        {
            // Fallback to old behavior if no screen assigned
            ProceedToLeaderboard();
        }
    }

    /**
     * Proceed to leaderboard logic (called from Win Screen "Home" button)
     */
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale
        ProceedToLeaderboard();
    }

    protected virtual void ProceedToLeaderboard()
    {
        string leaderboardName = PlayerPrefs.GetString("leaderboardname");

        if (leaderboardName == "")
        {
            // No name registered, store score temporarily and go to setup scene
            PlayerPrefs.SetString("last_score", $"time:{timeElapsed:F2}");
            SceneManager.LoadScene("LeaderboardSetup");
            return;
        }

        // Name exists, update leaderboard and go to main menu
        UpdateLeaderboard(leaderboardName, timeElapsed);
        SceneManager.LoadScene("MainMenu");
    }

    /**
     * Game Over Logic
     */
    public void GameOver()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (playerUI != null) playerUI.ToggleCrosshair(false);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public virtual void GoToLeaderboard()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("ShowLeaderboard", 1);

        // If we won (enemies cleared and player alive), save the score
        if (enemyCount <= 0 && areEnemiesSpawned && !isPlayerDead)
        {
            ProceedToLeaderboard();
        }
        else
        {
            // Otherwise just go to main menu
            SceneManager.LoadScene("MainMenu");
        }
    }

    /**
     * Updates the leaderboard with a new entry
     * @param playerName The player's name
     * @param score The score value (time for main mode)
     */
    protected void UpdateLeaderboard(string playerName, float score)
    {
        string leaderboard = PlayerPrefs.GetString("leaderboard");
        List<LeaderboardEntry> allEntries = ParseLeaderboardEntries(leaderboard);

        // Separate by type
        List<LeaderboardEntry> timeEntries = allEntries.Where(e => !e.isRounds).ToList();
        List<LeaderboardEntry> roundsEntries = allEntries.Where(e => e.isRounds).ToList();

        // Add new time entry
        timeEntries.Add(new LeaderboardEntry { name = playerName, score = score, isRounds = false });

        // Sort time entries (lower is better)
        timeEntries = timeEntries.OrderBy(e => e.score).Take(3).ToList();

        // Combine both types back together
        List<string> formattedEntries = new List<string>();

        // Format time entries as "name:score"
        formattedEntries.AddRange(timeEntries.Select(e => $"{e.name}:{e.score:F2}"));

        // Format rounds entries as "name:rounds:X"
        formattedEntries.AddRange(roundsEntries.Select(e => $"{e.name}:rounds:{(int)e.score}"));

        leaderboard = string.Join(",", formattedEntries);

        PlayerPrefs.SetString("leaderboard", leaderboard);
    }

    /**
     * Parses leaderboard entries, handling both old format (just numbers) and new format (name:score)
     */
    protected List<LeaderboardEntry> ParseLeaderboardEntries(string leaderboard)
    {
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        if (string.IsNullOrEmpty(leaderboard))
        {
            return entries;
        }

        string[] parts = leaderboard.Split(',');
        foreach (string part in parts)
        {
            if (string.IsNullOrEmpty(part)) continue;

            // Check if it's new format (name:score or name:rounds:X)
            if (part.Contains(":"))
            {
                string[] entryParts = part.Split(':');
                if (entryParts.Length >= 2)
                {
                    if (entryParts[1] == "rounds" && entryParts.Length >= 3)
                    {
                        // Round mode entry: "name:rounds:X"
                        if (int.TryParse(entryParts[2], out int rounds))
                        {
                            entries.Add(new LeaderboardEntry
                            {
                                name = entryParts[0],
                                score = rounds,
                                isRounds = true
                            });
                        }
                    }
                    else
                    {
                        // Time mode entry: "name:score"
                        if (float.TryParse(entryParts[1], out float score))
                        {
                            entries.Add(new LeaderboardEntry
                            {
                                name = entryParts[0],
                                score = score,
                                isRounds = false
                            });
                        }
                    }
                }
            }
            else
            {
                // Old format: just a number (time)
                if (float.TryParse(part, out float score))
                {
                    entries.Add(new LeaderboardEntry
                    {
                        name = "Unknown",
                        score = score,
                        isRounds = false
                    });
                }
            }
        }

        return entries;
    }

    /**
     * Helper class for leaderboard entries
     */
    protected class LeaderboardEntry
    {
        public string name;
        public float score;
        public bool isRounds;
    }

    /**
    * Starts the slow motion
    */
    public void StartSlowMotion(){
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }

    /**
    * Stops the slow motion
    */
    public void StopSlowMotion(){
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02F;
    }

    /**
     * Checks if the game is over
     * @return bool True if the game is over, false otherwise
     */
    public virtual bool IsGameOver(){
        return (enemyCount <= 0 && areEnemiesSpawned) || isPlayerDead;
    }

    /**
     * Called when the player dies
     */
    public void OnPlayerDeath(){
        GameOver();
    }

    public virtual void OnEnemyDeath(){
        enemyCount--;
    }
}
