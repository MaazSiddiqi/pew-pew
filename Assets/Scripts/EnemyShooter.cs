using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;
    public float fireRate = 1f;
    private float nextFireTime = 0f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    // Start is called before the first frame update
    void Start()
    {
        // Find player by tag if not assigned
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            // Look at player
            transform.LookAt(player);

            // Shoot
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            
            // Assign owner to prevent self-damage
            Bullet b = bullet.GetComponent<Bullet>();
            if (b != null)
            {
                b.owner = gameObject;
            }

            // Ignore collision with self (Physics layer backup)
            Collider enemyCollider = GetComponent<Collider>();
            Collider bulletCollider = bullet.GetComponent<Collider>();
            if (enemyCollider != null && bulletCollider != null)
            {
                Physics.IgnoreCollision(enemyCollider, bulletCollider);
            }
        }
    }
}
