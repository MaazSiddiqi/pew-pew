using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Interactable
{
    public float healAmount = 20f;

    void Start()
    {
        if (string.IsNullOrEmpty(promptMessage))
        {
            string binding = "E";
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                InputManager inputManager = player.GetComponent<InputManager>();
                if (inputManager != null)
                {
                    binding = inputManager.GetInteractBinding();
                }
            }
            promptMessage = $"Press {binding} to pick up Health";
        }
    }

    protected override void Interact()
    {
        // Find the player (assuming the player is the one interacting)
        // In a more complex system, BaseInteract could pass the interactor
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.Heal(healAmount);
                Debug.Log($"Healed player for {healAmount}");
                Destroy(gameObject);
            }
        }
    }
}
