using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int ammoAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        // Try to find WeaponManager on the object that hit us (or its children/parent)
        WeaponManager weaponManager = other.GetComponent<WeaponManager>();
        if (weaponManager == null)
        {
            weaponManager = other.GetComponentInChildren<WeaponManager>();
        }
        
        // If not found, maybe it's on the parent (like Player object)
        if (weaponManager == null)
        {
            weaponManager = other.GetComponentInParent<WeaponManager>();
        }

        if (weaponManager != null)
        {
            weaponManager.AddAmmo(ammoAmount);
            Debug.Log("Picked up ammo!");
            Destroy(gameObject);
        }
    }
}
