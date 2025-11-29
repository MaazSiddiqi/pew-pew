using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject player;

    public float speed = 5f;
    public float walkingRange = 100f; // Distance at which enemy will start targeting the player
    public float shootingRange = 10f;
    public float attackCooldown = 1f;

    public float health = 100f;

    [Header("Death Effect")]
    public GameObject deathEffectPrefab; // CFXR Magic Poof prefab

    private bool isDead = false;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = speed;
        }
        
        if (GameManager.instance != null)
        {
            GameManager.instance.OnEnemySpawned();
        }
    }

    void OnDestroy()
    {
        if (GameManager.instance != null && !isDead)
        {
            GameManager.instance.enemyCount--;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null){
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Attack if within shooting range
        if (distanceToPlayer <= shootingRange)
        {
            // Stop moving to shoot
            if (agent != null && agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
            
            // Look at player
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
            
            Attack();
        }
        // Chase if outside shooting range but within walking range
        else if (distanceToPlayer <= walkingRange)
        {
            ChasePlayer();
        }
        // Idle if too far
        else
        {
            if (agent != null && agent.isOnNavMesh)
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
        if (isDead) return;
        isDead = true;

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
        if(agent.isOnNavMesh){
            agent.SetDestination(player.transform.position);
        }
    }

    public void Attack(){
        // attack
    }
}
