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

    public int enemyCount = 0;
    public bool isPlayerDead = false;

    public float timeElapsed = 0f;

    public string gameMode = "Classic";
    public bool areEnemiesSpawned = false;

    void Start(){
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
    }

    void Update(){
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

    public virtual void OnEnemySpawned(){
        enemyCount++;
        areEnemiesSpawned = true;
    }

    /**
    * Ends the game
    */
    public void EndGame(){
        SceneManager.LoadScene("MainMenu");

        string leaderboard = PlayerPrefs.GetString("leaderboard");
        if (leaderboard == "")
        {
            leaderboard = timeElapsed.ToString("F2");
        }
        else
        {
            List<float> times = leaderboard.Split(',').Select(float.Parse).ToList();
            times.Add(timeElapsed);
            times.Sort();
            leaderboard = string.Join(",", times.Take(3).Select(t => t.ToString("F2")));
        }

        PlayerPrefs.SetString("leaderboard", leaderboard);
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
        EndGame();
    }

    public virtual void OnEnemyDeath(){
        enemyCount--;
    }
}
