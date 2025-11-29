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
    public TextMeshProUGUI leaderboardText;

    void Start()
    {
        string leaderboard = PlayerPrefs.GetString("leaderboard");
        if (leaderboard == "")
        {
            leaderboardText.text = "No times yet!";
        }
        else
        {
            List<float> times = leaderboard.Split(',').Select(float.Parse).ToList();
            times.Sort();
            leaderboardText.text = "Top 3 times:\n" + string.Join("\n", times.Take(3).Select(t => t.ToString("F2")));
        }
    }

    public void handleStartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void handleShowLeaderboard()
    {
        mainMenuPanel.SetActive(false);
        leaderboardPanel.SetActive(true);
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
