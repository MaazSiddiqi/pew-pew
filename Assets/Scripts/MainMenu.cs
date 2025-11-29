using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject leaderboardPanel;
    public TextMeshProUGUI mainContentText;
    public TextMeshProUGUI arcadeContentText;

    void Start()
    {
        UpdateLeaderboards();
    }

    /**
     * Updates both MAIN and ARCADE leaderboard displays
     */
    public void UpdateLeaderboards()
    {
        string leaderboard = PlayerPrefs.GetString("leaderboard");
        List<LeaderboardEntry> entries = ParseLeaderboardEntries(leaderboard);

        // Separate entries by type
        List<LeaderboardEntry> mainEntries = entries.Where(e => !e.isRounds).ToList();
        List<LeaderboardEntry> arcadeEntries = entries.Where(e => e.isRounds).ToList();

        // Update MAIN leaderboard (time-based)
        if (mainContentText != null)
        {
            if (mainEntries.Count == 0)
            {
                mainContentText.text = "1.\n2.\n3.";
            }
            else
            {
                // Sort by time (lower is better)
                mainEntries = mainEntries.OrderBy(e => e.score).Take(3).ToList();
                string[] lines = new string[3];
                for (int i = 0; i < 3; i++)
                {
                    if (i < mainEntries.Count)
                    {
                        lines[i] = $"{i + 1}. {mainEntries[i].name} - {mainEntries[i].score:F2}s";
                    }
                    else
                    {                        lines[i] = $"{i + 1}.";
                    }
                }
                mainContentText.text = string.Join("\n", lines);
            }
        }

        // Update ARCADE leaderboard (rounds-based)
        if (arcadeContentText != null)
        {
            if (arcadeEntries.Count == 0)
            {
                arcadeContentText.text = "1.\n2.\n3.";
            }
            else
            {
                // Sort by rounds (higher is better)
                arcadeEntries = arcadeEntries.OrderByDescending(e => e.score).Take(3).ToList();
                string[] lines = new string[3];
                for (int i = 0; i < 3; i++)
                {
                    if (i < arcadeEntries.Count)
                    {
                        lines[i] = $"{i + 1}. {arcadeEntries[i].name} - Round {(int)arcadeEntries[i].score}";
                    }
                    else
                    {
                        lines[i] = $"{i + 1}.";
                    }
                }
                arcadeContentText.text = string.Join("\n", lines);
            }
        }
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

    public void handleStartGame()
    {
        PlayerPrefs.SetString("GameMode", "Classic");
        SceneManager.LoadScene(1);
    }

    public void handleStartArcadeGame()
    {
        PlayerPrefs.SetString("GameMode", "Arcade");
        SceneManager.LoadScene(2);
    }

    public void handleShowLeaderboard()
    {
        mainMenuPanel.SetActive(false);
        leaderboardPanel.SetActive(true);
        UpdateLeaderboards(); // Refresh leaderboards when showing the panel
    }

    public void handleQuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void handleBackToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        leaderboardPanel.SetActive(false);
    }

}
