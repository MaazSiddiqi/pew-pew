using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;

    public float damage = 10f;

    public GameObject owner;

    // Start is called before the first frame update
    void Start()
    {
        // Destroy bullet after 5 seconds to keep hierarchy clean
        Destroy(gameObject, 5f);
        Debug.Log($"Bullet spawned at: {transform.position}");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore collision with owner
        if (owner != null && (other.gameObject == owner || other.transform.IsChildOf(owner.transform)))
        {
            return;
        }

        Debug.Log($"Bullet hit: {other.name}");
        // Check if we hit a TargetDummy
        TargetDummy dummy = other.GetComponent<TargetDummy>();
        if (dummy != null)
        {
            dummy.TakeDamage(damage);
            Destroy(gameObject); // Destroy bullet on impact
            return;
        }

        // Check if we hit an Enemy (EnemyAI)
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Check if we hit the Player
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // Optional: Destroy bullet on hitting walls/environment (if they have a specific tag or layer)
        // For now, let's just destroy on enemy hit or timeout
    }
}
