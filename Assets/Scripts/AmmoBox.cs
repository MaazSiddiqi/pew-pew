using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : Interactable
{
    public int ammoAmount = 1;

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
            promptMessage = $"Press {binding} to pick up Ammo";
        }
    }

    protected override void Interact()
    {
        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Try to find WeaponManager on the player or its children (the gun)
            WeaponManager weaponManager = player.GetComponentInChildren<WeaponManager>();
            
            if (weaponManager != null)
            {
                weaponManager.AddAmmo(ammoAmount);
                Debug.Log("Picked up ammo!");
                Destroy(gameObject);
            }
        }
    }
}
