using UnityEngine;
using UnityEngine.SceneManagement; // For loading the game over scene

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health the player can have
    private int currentHealth;

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
        Debug.Log("collided");
        if (other.gameObject.CompareTag("DamageBox"))
        { TakeDamage(100); }
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
        }
        Debug.Log("Current Health: " + currentHealth);
    }

    // Game over method
    private void GameOver()
    {
        // Load the Game Over scene (ensure you have a scene named "GameOver")
        // SceneManager.LoadScene("GameOver");
        Debug.Log("GameOver");
    }
}