using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] public int maxHealth = 100; // Maximum health the player can have
    [SerializeField] TextMeshPro healthBarComponent;

    private int currentHealth;    

    public override void OnNetworkSpawn()
    {
        currentHealth = maxHealth; // Initialize current health to max health
    }
    public void HandleCollision(int healthModifier) {
        if (healthModifier < 0) {
            TakeDamage(healthModifier);
        } else {
            RestoreHealth(healthModifier);
        }
    }
    public void TakeDamage(int damage)
    {
        SetHealth(Mathf.Max(currentHealth + damage, 0));
        if (currentHealth <= 0)
        {
            Debug.Log("Player has died!");
            GameOver();
            return;
        }
    }
     private void SetHealth(int health) {
        currentHealth = health;
        float ratioHealth = (float)currentHealth / maxHealth;
        healthBarComponent.text = Mathf.RoundToInt(ratioHealth * 100) + "%";
    }
    public int AddHealth(int amount)
    {
        // Prevent exceeding max health
        return Mathf.Min(currentHealth + amount, maxHealth);
    }

     private void RestoreHealth(int healthRestored) {
        int targetHealth = AddHealth(healthRestored);
        SetHealth(targetHealth);
    }
    private void GameOver()
    {
        SceneManager.LoadScene("Game Over");
    }
}
