using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject player;

    public float speed = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    public float health = 100f;

    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        GameManager.instance.enemyCount++;
    }

    void OnDestroy()
    {
        GameManager.instance.enemyCount--;
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null){
            return;
        }

        ChasePlayer();

        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange){
            Attack();
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
        GameManager.instance.OnEnemyDeath();
        Destroy(gameObject);
    }

    public void ChasePlayer(){
        agent.SetDestination(player.transform.position);
    }

    public void Attack(){
        // attack
    }
}
