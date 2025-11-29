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

    [Header("Hit Effect")]
    public float bounceScale = 1.1f; // How much to scale up on hit
    public float bounceDuration = 0.15f; // How long the bounce effect lasts
    public Color hitTintColor = Color.red; // Color tint when hit

    private NavMeshAgent agent;
    private Vector3 originalScale;
    private Coroutine bounceCoroutine;
    private Renderer enemyRenderer;
    private MaterialPropertyBlock materialPropertyBlock;
    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        originalScale = transform.localScale; // Store original scale

        // Get renderer and store original color (check both self and children)
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer == null)
        {
            enemyRenderer = GetComponentInChildren<Renderer>();
        }

        if (enemyRenderer != null && enemyRenderer.material != null)
        {
            originalColor = enemyRenderer.material.color;
            materialPropertyBlock = new MaterialPropertyBlock();
        }

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

        // Trigger bounce effect
        if (bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
        }
        bounceCoroutine = StartCoroutine(BounceEffect());

        if(health <= 0){
            Die();
        }
    }

    /**
     * Creates a bounce effect by scaling up then lerping back, with red tint fade
     */
    private IEnumerator BounceEffect()
    {
        float elapsed = 0f;
        Vector3 targetScale = originalScale * bounceScale;

        // Scale up and tint red (first half of duration)
        while (elapsed < bounceDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (bounceDuration * 0.5f);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);

            // Fade in red tint
            if (enemyRenderer != null && materialPropertyBlock != null)
            {
                Color currentColor = Color.Lerp(originalColor, hitTintColor, t);
                materialPropertyBlock.SetColor("_Color", currentColor);
                enemyRenderer.SetPropertyBlock(materialPropertyBlock);
            }

            yield return null;
        }

        // Scale back down and fade out tint (second half of duration)
        elapsed = 0f;
        while (elapsed < bounceDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (bounceDuration * 0.5f);
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);

            // Fade out red tint
            if (enemyRenderer != null && materialPropertyBlock != null)
            {
                Color currentColor = Color.Lerp(hitTintColor, originalColor, t);
                materialPropertyBlock.SetColor("_Color", currentColor);
                enemyRenderer.SetPropertyBlock(materialPropertyBlock);
            }

            yield return null;
        }

        // Ensure we're back to original scale and color
        transform.localScale = originalScale;
        if (enemyRenderer != null && materialPropertyBlock != null)
        {
            materialPropertyBlock.SetColor("_Color", originalColor);
            enemyRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        bounceCoroutine = null;
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
