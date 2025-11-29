using System.Collections;
using System.Collections.Generic;
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

    void Start(){
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update(){
        timeElapsed += Time.deltaTime;
        timeElapsedText.text = timeElapsed.ToString("F2");

        if (enemyCount <= 0)
        {
            winZone.SetActive(true);
            promptText.gameObject.SetActive(true);
            promptText.text = "Enemies cleared! Head over to the end zone to finish!";
            promptText.color = Color.green;
        } else {
            winZone.SetActive(false);
        }
    }

    /**
    * Ends the game
    */
    public void EndGame(){
        SceneManager.LoadScene("MainMenu");
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
    * Called when the player dies
    */
    public void OnPlayerDeath(){
        EndGame();
    }

    public void OnEnemyDeath(){
        enemyCount--;
    }
}
