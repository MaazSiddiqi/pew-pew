using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;

    public float damage = 10f;

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
        Debug.Log($"Bullet hit: {other.name}");
        // Check if we hit a TargetDummy
        TargetDummy enemy = other.GetComponent<TargetDummy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); // Destroy bullet on impact
        }
        // Optional: Destroy bullet on hitting walls/environment (if they have a specific tag or layer)
        // For now, let's just destroy on enemy hit or timeout
    }
}
