using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public TMPro.TextMeshProUGUI ammoText;

    public int clips = 3;
    public int bulletsPerClip = 10;

    public int bullets = 0;
    public float reloadTime = 1f;
    public float damage = 10f;
    public float fireRate = 0.1f;
    private float lastShotTime = 0f;

    public GameObject currentMagazine;
    public GameObject magazinePrefab;
    public Transform magHolder;
    private Queue<GameObject> droppedMags = new Queue<GameObject>();
    private CharacterController playerController;

    private bool isReloading = false;

    // Start is called before the first frame update
    void Start()
    {
        bullets = bulletsPerClip;
        UpdateAmmoUI();
        playerController = GetComponentInParent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Shoot(){
        if (isReloading) return; // Can't shoot while reloading
        if (Time.time < lastShotTime + fireRate) return; // Cooldown

        lastShotTime = Time.time;

        if(bullets > 0){
            bullets--;
            UpdateAmmoUI();
            Debug.Log("Shooting");
            // Use firePoint if available, otherwise fallback to transform
            Transform spawnPoint = firePoint != null ? firePoint : transform;
            GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
            bullet.GetComponent<Bullet>().damage = damage;
            
            // Ignore collision between bullet and player
            if (playerController != null)
            {
                Collider bulletCol = bullet.GetComponent<Collider>();
                if (bulletCol != null) Physics.IgnoreCollision(bulletCol, playerController);
            }
        }
    }

    public void Reload(){
        if (isReloading || bullets == bulletsPerClip || clips == 0)
        {
            return;
        }

        StartCoroutine(ReloadCoroutine());
    }

    public IEnumerator ReloadCoroutine() {
        isReloading = true;
        Debug.Log("Reloading");

        // 1. Drop current magazine
        if (currentMagazine != null)
        {
            currentMagazine.transform.SetParent(null); // Detach from gun
            
            // Enable physics
            Rigidbody rb = currentMagazine.GetComponent<Rigidbody>();
            if (rb == null) rb = currentMagazine.AddComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;

            // Add collider if missing (optional safety check)
            Collider col = currentMagazine.GetComponent<Collider>();
            if (col == null) 
            {
                col = currentMagazine.AddComponent<BoxCollider>();
            }

            // Ignore collision with player
            if (playerController != null && col != null)
            {
                Physics.IgnoreCollision(col, playerController);
            }

            // Add some torque to make it tumble and land flat
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

            // Manage queue
            droppedMags.Enqueue(currentMagazine);
            if (droppedMags.Count > 2)
            {
                GameObject oldMag = droppedMags.Dequeue();
                Destroy(oldMag);
            }
        }

        // 2. Spawn new magazine below
        if (magazinePrefab != null && magHolder != null)
        {
            // Spawn slightly below the holder
            Vector3 spawnPos = magHolder.position + (Vector3.down * 0.5f); 
            GameObject newMag = Instantiate(magazinePrefab, spawnPos, magHolder.rotation);
            
            // Disable physics for the new mag while it's animating
            Rigidbody newRb = newMag.GetComponent<Rigidbody>();
            if (newRb != null) newRb.isKinematic = true;

            // Ignore collision with player (just in case)
            Collider newCol = newMag.GetComponent<Collider>();
            if (newCol != null && playerController != null)
            {
                Physics.IgnoreCollision(newCol, playerController);
            }

            newMag.transform.SetParent(magHolder);

            // 3. Animate upwards
            float elapsed = 0f;
            float duration = reloadTime * 0.5f; // Use half of reload time for animation
            Vector3 startLocalPos = newMag.transform.localPosition;
            
            while (elapsed < duration)
            {
                newMag.transform.localPosition = Vector3.Lerp(startLocalPos, Vector3.zero, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            newMag.transform.localPosition = Vector3.zero;
            currentMagazine = newMag; // Update reference
        }
        else
        {
            // Fallback if no prefabs assigned
            yield return new WaitForSeconds(reloadTime);
        }

        bullets = bulletsPerClip;
        clips--;
        UpdateAmmoUI();
        isReloading = false;
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"Ammo: {bullets} / {bulletsPerClip} Clips: {clips}";
            Debug.Log($"Updated UI: {ammoText.text}");
        }
        else
        {
            Debug.LogError("Ammo Text is NULL in WeaponManager!");
        }
    }

    public void AddAmmo(int amount)
    {
        clips += amount;
        UpdateAmmoUI();
    }

}

