using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public TMPro.TextMeshProUGUI healthText;
    public AudioClip hurtSound;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Current Health: {currentHealth}");
        UpdateHealthUI();

        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"HP: {Mathf.Ceil(currentHealth)}";
            
            // Change color based on health percentage
            float percentage = currentHealth / maxHealth;

            if (percentage > 0.75f)
            {
                healthText.color = Color.green;
            }
            else if (percentage > 0.5f)
            {
                healthText.color = Color.yellow;
            }
            else if (percentage > 0.25f)
            {
                healthText.color = new Color(1f, 0.64f, 0f); // Orange
            }
            else
            {
                healthText.color = Color.red;
            }
        }
        else
        {
            Debug.LogError($"Health Text is NULL on {gameObject.name}!");
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log($"Player healed {amount}. Current Health: {currentHealth}");
        UpdateHealthUI();
    }

    void Die()
    {
        Debug.Log("Player Died!");
        // Handle player death (e.g., restart level, show game over screen)
        // For now, just disable the player or pause
        // gameObject.SetActive(false); 
    }
}
