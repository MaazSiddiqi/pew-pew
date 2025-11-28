using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject player;

    public float speed = 5f;
    public float detectionRange = 10f; // Distance at which enemy will start targeting the player
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    public float health = 100f;

    [Header("Death Effect")]
    public GameObject deathEffectPrefab; // CFXR Magic Poof prefab

    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        if (GameManager.instance != null)
        {
            GameManager.instance.enemyCount++;
        }
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnEnemyDeath();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null){
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Only chase player if within detection range
        if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();

            // Attack if within attack range
            if (distanceToPlayer <= attackRange){
                Attack();
            }
        }
        else
        {
            // Stop moving if player is out of range
            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.ResetPath();
            }
        }
    }

    /**
    * Takes damage
    * Called when a bullet hits the enemy
    * @param damage The amount of damage to take
    */
    public void TakeDamage(float damage){
        health -= damage;
        if(health <= 0){
            Die();
        }
    }

    public void Die(){
        // Spawn death effect if prefab is assigned
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.OnEnemyDeath();
        }
        Destroy(gameObject);
    }

    public void ChasePlayer(){
        agent.SetDestination(player.transform.position);
    }

    public void Attack(){
        // attack
    }
}
