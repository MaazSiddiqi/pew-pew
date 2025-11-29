using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class LeaderboardSetup : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField nameInputField;
    public TextMeshProUGUI scoreDisplayText;
    public UnityEngine.UI.Button submitButton;
    public UnityEngine.UI.Button backButton;

    private string scoreType;
    private string scoreValue;

    void Start()
    {
        // Get the stored score
        string lastScore = PlayerPrefs.GetString("last_score");

        if (string.IsNullOrEmpty(lastScore))
        {
            // No score found, go back to main menu
            SceneManager.LoadScene("MainMenu");
            return;
        }

        // Parse the score
        string[] parts = lastScore.Split(':');
        if (parts.Length >= 2)
        {
            scoreType = parts[0];
            scoreValue = parts[1];

            // Display appropriate message
            if (scoreType == "time")
            {
                if (float.TryParse(scoreValue, out float time))
                {
                    scoreDisplayText.text = $"Your time: {time:F2} seconds";
                }
            }
            else if (scoreType == "rounds")
            {
                if (int.TryParse(scoreValue, out int rounds))
                {
                    scoreDisplayText.text = $"Rounds completed: {rounds}";
                }
            }
        }

        // Set up button listeners
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitName);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackToMain);
        }

        // Set up input field submit listener
        if (nameInputField != null)
        {
            nameInputField.onSubmit.AddListener((string value) => OnSubmitName());
        }
    }

    public void OnSubmitName()
    {
        string playerName = nameInputField != null ? nameInputField.text : "";

        if (string.IsNullOrWhiteSpace(playerName))
        {
            // Name is empty, don't proceed
            return;
        }

        // Register the name
        PlayerPrefs.SetString("leaderboardname", playerName);

        // Update leaderboard
        UpdateLeaderboard(playerName);

        // Clear temporary score
        PlayerPrefs.DeleteKey("last_score");

        // Go to main menu
        SceneManager.LoadScene("MainMenu");
    }

    public void OnBackToMain()
    {
        // Clear temporary score without updating leaderboard
        PlayerPrefs.DeleteKey("last_score");

        // Go to main menu
        SceneManager.LoadScene("MainMenu");
    }

    private void UpdateLeaderboard(string playerName)
    {
        string leaderboard = PlayerPrefs.GetString("leaderboard");
        List<LeaderboardEntry> allEntries = ParseLeaderboardEntries(leaderboard);

        // Separate by type
        List<LeaderboardEntry> timeEntries = allEntries.Where(e => !e.isRounds).ToList();
        List<LeaderboardEntry> roundsEntries = allEntries.Where(e => e.isRounds).ToList();

        if (scoreType == "time")
        {
            if (float.TryParse(scoreValue, out float time))
            {
                timeEntries.Add(new LeaderboardEntry { name = playerName, score = time, isRounds = false });
                timeEntries = timeEntries.OrderBy(e => e.score).Take(3).ToList(); // Lower is better for time
            }
        }
        else if (scoreType == "rounds")
        {
            if (int.TryParse(scoreValue, out int rounds))
            {
                roundsEntries.Add(new LeaderboardEntry { name = playerName, score = rounds, isRounds = true });
                roundsEntries = roundsEntries.OrderByDescending(e => e.score).Take(3).ToList(); // Higher is better for rounds
            }
        }

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
    private List<LeaderboardEntry> ParseLeaderboardEntries(string leaderboard)
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
    private class LeaderboardEntry
    {
        public string name;
        public float score;
        public bool isRounds;
    }
}

