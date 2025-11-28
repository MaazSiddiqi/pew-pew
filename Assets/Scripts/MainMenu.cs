using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void handleStartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void handleQuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
