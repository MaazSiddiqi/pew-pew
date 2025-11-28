using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    void Awake(){
        instance = this;
    }

    void Update(){
        if(IsGameOver()){
            EndGame();
        }
    }

    /**
    * Ends the game
    */
    public void EndGame(){
        Debug.Log("Game Over");
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
    public bool IsGameOver(){
        return false;
    }

    /**
    * Called when the player dies
    */
    public void OnPlayerDeath(){
        EndGame();
    }

    public void OnEnemyDeath(){
        // decrease the enemy count
        // eventually call end game
    }
}
