using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEnd : MonoBehaviour
{

    public GameObject player;

    private float timeInZone = 0f;
    private bool playerInZone = false;
    private const float REQUIRED_TIME_IN_ZONE = 2f; // 2 seconds

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }

        // If player is in zone, increment timer
        if (playerInZone)
        {
            timeInZone += Time.deltaTime;

            // If player has been in zone for required time, end the game
            if (timeInZone >= REQUIRED_TIME_IN_ZONE)
            {
                if (GameManager.instance != null)
                {
                    GameManager.instance.EndGame();
                }
                // Reset to prevent multiple calls
                playerInZone = false;
                timeInZone = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the endzone
        if (other.gameObject == player || other.CompareTag("Player"))
        {
            Debug.Log("Player entered win zone");
            playerInZone = true;
            timeInZone = 0f; // Reset timer when entering
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Keep player marked as in zone while they're still in it
        if (other.gameObject == player || other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset timer when player leaves the zone
        if (other.gameObject == player || other.CompareTag("Player"))
        {
            Debug.Log("Player exited win zone");
            playerInZone = false;
            timeInZone = 0f;
        }
    }
}
