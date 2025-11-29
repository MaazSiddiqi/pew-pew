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

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log($"Enemy {enemy.gameObject.name} took {damage} damage");
            return;
        }

        // // Check if we hit the Player
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.Log($"Player {player.gameObject.name} took {damage} damage");
            return;
        }

        Destroy(gameObject);
    }
}
