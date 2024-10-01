using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement; // For loading the game over scene

public class PlayerHealth : MonoBehaviour {

    [SerializeField] public int maxHealth = 100; // Maximum health the player can have
    public int currentHealth;
    [SerializeField] TextMeshPro healthBarComponent;

    void Start()
    {
        currentHealth = maxHealth; // Initialize current health to max health
    }

    // Method to add HP
    public void AddHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) // Prevent exceeding max health
        {
            currentHealth = maxHealth;
        }
        Debug.Log("Current Health: " + currentHealth);
    }
    public void OnTriggerEnter(Collider other)

    {
        GameObject otherGameObject = other.GameObject();
        int damageDealt = otherGameObject.GetComponent<DamageComponent>().damageDealt;
        Debug.Log("collided with " + otherGameObject.name);
        Debug.Log("Damage to deal = " + damageDealt);
        if (other.gameObject.CompareTag("DamageBox"))
        {
            TakeDamage(damageDealt);
        }
    }
    // Method to take damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) // Check if health reaches 0 or below
        {
            currentHealth = 0;
            Debug.Log("Player has died!");
            GameOver(); // Call game over method
            return;
        }
        float ratioHealth = (float)currentHealth / maxHealth;
        healthBarComponent.text = Mathf.RoundToInt(ratioHealth * 100) + "%";
        Debug.Log("Current Health: " + currentHealth);
    }

    // Game over method
    private void GameOver()
    {
        SceneManager.LoadScene("Game Over");
    }
}